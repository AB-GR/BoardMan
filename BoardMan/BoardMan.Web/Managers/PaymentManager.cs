using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BoardMan.Web.Managers
{
	public interface IPaymentManager
	{
		Task<ValidatePaymentResponse> ValidatePaymentAsync(ValidatePaymentRequest request);

		Task<PaymentIntentResponse> CreatePaymentIntentAsync(PaymentIntentRequest request);

		Task<PaymentResult> ProcessPaymentAsync(PaymentSuccessRequest request);
	}

	public class PaymentManager : IPaymentManager
	{
		private readonly BoardManDbContext dbContext;
		private readonly ILogger<PaymentManager> logger;
		private readonly IPlanManager planManager;
		private readonly IPaymentService paymentService;
		private readonly IMapper mapper;
		private readonly UserManager<AppUser> userManager;
		private readonly IWorkspaceManager workspaceManager;

		public PaymentManager(IPlanManager planManager, IPaymentService paymentService, BoardManDbContext dbContext, ILogger<PaymentManager> logger, IMapper mapper, UserManager<AppUser> userManager, IWorkspaceManager workspaceManager)
		{
			this.planManager = planManager;
			this.paymentService = paymentService;
			this.dbContext = dbContext;
			this.logger = logger;
			this.mapper = mapper;
			this.userManager = userManager;
			this.workspaceManager = workspaceManager;
		}

		public async Task<PaymentIntentResponse> CreatePaymentIntentAsync(PaymentIntentRequest request)
		{			
			request.ProductName = "Boardman";
			request.BusinessName = "ABTech";
			var plan = await this.planManager.GetPlanAsync(request.PlanId);

			if (plan == null)
				throw new InsufficientDataToProcessException($"Plan {request.PlanId} does not exist.");			

			request.PlanName = plan.Name;
			request.PlanDescription = plan.Description;

			return await this.paymentService.CreatePaymentIntentAsync(request).ConfigureAwait(false);
		}

		public async Task<PaymentResult> ProcessPaymentAsync(PaymentSuccessRequest request)
		{
			var result = new PaymentResult { UserDetails = new UserResult() };
			var paymentIntentVM = await this.paymentService.GetPaymentIntentAsync(request).ConfigureAwait(false);
			paymentIntentVM.BillingDetails = request.BillingDetails;
			paymentIntentVM.TransactedById = request.UserId;

			using (ValueLock.Get(paymentIntentVM.PaymentReference).Lock())
			{
				var transaction = await this.dbContext.PaymentTransactions
					.Include(x => x.TransactedBy).FirstOrDefaultAsync(p => p.PaymentReference == paymentIntentVM.PaymentReference).ConfigureAwait(false);

				if(transaction == null)
				{
					transaction = await PopulateTransactionAsync(paymentIntentVM).ConfigureAwait(false);					
					if(paymentIntentVM.TransactedById.IsNullOrEmpty())
					{
						var user = await this.userManager.FindByEmailAsync(paymentIntentVM.BillingDetails.UserEmail).ConfigureAwait(false);
						if(user != null)
						{
							transaction.TransactedById = user.Id;
							transaction.TransactedBy = user;							
						}
						else
						{
							result.UserDetails = await CreateNewUserAsync(paymentIntentVM).ConfigureAwait(false);
							if (result.UserDetails.CreateResult.Succeeded)
							{
								transaction.TransactedById = result.UserDetails.User.Id;
								transaction.TransactedBy = result.UserDetails.User;
							}
							else
							{
								transaction.Status = PaymentStatus.Invalid;
								transaction.StatusReason = "New user could not be created";
								logger.LogWarning($"For payment reference {paymentIntentVM.PaymentReference} a new user could not be created because of this error: {result.UserDetails.CreateResult.ErrorsString()}");
							}
						}						
					}
					else
					{
						result.UserDetails.UserIsLoggedIn = true;
					}

					await ProcessTransactionAsync(transaction).ConfigureAwait(false);
				}

				result.PaymentStatus = transaction.Status;
				return result;
			}
		}

		public async Task<ValidatePaymentResponse> ValidatePaymentAsync(ValidatePaymentRequest request)
		{
			var response = new ValidatePaymentResponse
			{
				CanProceed = true,
				ExistingUser = false
			};
			
			// Check for a valid user
			AppUser user;
			if(!request.UserId.IsNullOrEmpty())
			{
				user = await this.userManager.FindByIdAsync(request.UserId.GetValueOrDefault().ToString()).ConfigureAwait(false);
			}
			else if(!string.IsNullOrWhiteSpace(request.UserEmail))
			{
				user = await this.userManager.FindByEmailAsync(request.UserEmail).ConfigureAwait(false);
				if(user != null)
				{
					response.ExistingUser = true;
					response.Message = $"User with email address {user?.Email} already exists, do you wish to continue payment with the existing account ?";
				}				
			}
			else
			{
				throw new InsufficientDataToProcessException("User identifier not provided");
			}

			var plan = await dbContext.Plans.FindAsync(request.PlanId).ConfigureAwait(false);
			if (plan == null)
			{
				throw new InsufficientDataToProcessException("Plan does not exist");
			}

			if (user != null)
			{
				if (await dbContext.Subscriptions.AnyAsync(x => x.OwnerId == user.Id && x.ExpireAt > DateTime.UtcNow && x.PaymentTrasaction.PlanId == plan.Id).ConfigureAwait(false))
				{
					response.CanProceed = false;
					response.Message = $"User with email address {user.Email} has already purchased the plan";
				}
			}

			return response;
		}
		
		private async Task<UserResult> CreateNewUserAsync(PaymentTransaction paymentIntentVM)
		{
			var user = new AppUser
			{
				FirstName = paymentIntentVM.BillingDetails.UserFirstName,
				LastName = paymentIntentVM.BillingDetails.UserLastName,
				Email = paymentIntentVM.BillingDetails.UserEmail,
				UserName = paymentIntentVM.BillingDetails.UserEmail
			};

			var result = await userManager.CreateAsync(user, paymentIntentVM.BillingDetails.Password).ConfigureAwait(false);

			return new UserResult
			{
				User = user,
				CreateResult = result
			};
		}

		private async Task<DbPaymentTransaction> PopulateTransactionAsync(PaymentTransaction paymentIntent)
		{
			var transaction = this.mapper.Map<DbPaymentTransaction>(paymentIntent);
			var error = new StringBuilder(paymentIntent.Errors);

			var plan = await dbContext.Plans.FindAsync(paymentIntent.PlanId).ConfigureAwait(false);
			if (plan != null)
			{
				transaction.PlanId = paymentIntent.PlanId;
				transaction.Plan = plan;
			}
			else
				error.AppendLine($"Plan {paymentIntent.PlanId} does not exist.");

			if (!paymentIntent.TransactedById.IsNullOrEmpty())
			{
				var user = await dbContext.Users.FindAsync(paymentIntent.TransactedById).ConfigureAwait(false);
				if (user != null)
				{
					transaction.TransactedById = user.Id;
					transaction.TransactedBy = user;
				}
				else
					error.AppendLine($"User {paymentIntent.TransactedById} does not exist.");
			}
			else if(!string.IsNullOrWhiteSpace(paymentIntent.BillingDetails.UserEmail))
			{
				// Try finding an existing user by email & assign
				var user = await userManager.FindByEmailAsync(paymentIntent.BillingDetails.UserEmail);
				if (user != null)
				{
					transaction.TransactedById = user.Id;
					transaction.TransactedBy = user;
				}

			}

			if (error.Length > 0)
			{
				transaction.Status = PaymentStatus.Invalid;
				transaction.StatusReason = error.ToString();
			}

			return transaction;
		}		

		private async Task ProcessTransactionAsync(DbPaymentTransaction transaction)
		{
			try
			{
				dbContext.PaymentTransactions.Add(transaction);
				await dbContext.SaveChangesAsync().ConfigureAwait(false);				
				DbSubscription? subscription = null;

				if (transaction.Status == PaymentStatus.CanBeProcessed)
				{
					if (transaction.Plan.Expired)
					{
						transaction.Status = PaymentStatus.PlanExpired;
					}
					else if (transaction.Plan.Cost > transaction.FinalCost)
					{
						transaction.Status = PaymentStatus.AmountNotMatched;
						transaction.StatusReason += $"Expected: {transaction.Plan.Cost}, paid: {transaction.FinalCost}.\r\n";
					}

					if (transaction.Status == PaymentStatus.CanBeProcessed)
					{
						// Create new subscription & new workspace
						subscription = new DbSubscription
						{
							Name = $"Susbscription for the plan {transaction.Plan.Name}",
							Owner = transaction.TransactedBy,
							PaymentTrasaction = transaction,
							StartedAt = DateTime.UtcNow,
							ExpireAt =  transaction.Plan.PlanType == PlanType.Monthly ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(365)
						};
						dbContext.Subscriptions.Add(subscription);
						transaction.Status = PaymentStatus.Processed;
						transaction.StatusReason = Enum.GetName(typeof(PaymentStatus), PaymentStatus.Processed);
					}

					await dbContext.SaveChangesAsync().ConfigureAwait(false);
					if(transaction.Status == PaymentStatus.Processed)
					{
						await workspaceManager.CreateOrUpdateWorskpaceAsync(transaction.TransactedById.GetValueOrDefault(), subscription?.Id);
					}
				}
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, ex.Message);
				throw;
			}
		}
	}
}
