﻿using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Text;

namespace BoardMan.Web.Managers
{
	public interface IPaymentManager
	{
		Task<PaymentIntentResponseVM> CreatePaymentIntentAsync(PaymentIntentRequestVM request);

		Task<PaymentStatus> ProcessPaymentAsync(PaymentSuccessRequestVM request);
	}

	public class PaymentManager : IPaymentManager
	{
		private readonly BoardManDbContext dbContext;
		private readonly ILogger<PaymentManager> logger;
		private readonly IPlanManager planManager;
		private readonly IPaymentService paymentService;
		private readonly IMapper mapper;

		public PaymentManager(IPlanManager planManager, IPaymentService paymentService, BoardManDbContext dbContext, ILogger<PaymentManager> logger, IMapper mapper)
		{
			this.planManager = planManager;
			this.paymentService = paymentService;
			this.dbContext = dbContext;
			this.logger = logger;
			this.mapper = mapper;
		}

		public async Task<PaymentIntentResponseVM> CreatePaymentIntentAsync(PaymentIntentRequestVM request)
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

		public async Task<PaymentStatus> ProcessPaymentAsync(PaymentSuccessRequestVM request)
		{
			var paymentIntentVM = await this.paymentService.GetPaymentIntentAsync(request).ConfigureAwait(false);

			using (ValueLock.Get(paymentIntentVM.PaymentReference).Lock())
			{
				var transaction = await this.dbContext.PaymentTransactions
					.Include(x => x.TransactedBy).FirstOrDefaultAsync(p => p.PaymentReference == paymentIntentVM.PaymentReference).ConfigureAwait(false);

				if(transaction == null)
				{
					transaction = await PopulateTransactionAsync(paymentIntentVM).ConfigureAwait(false);
					await ProcessTransactionAsync(transaction).ConfigureAwait(false);
				}

				return transaction.Status;
			}
		}

		private async Task<DbPaymentTransaction> PopulateTransactionAsync(PaymentIntentVM paymentIntent)
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

			var user = await dbContext.Users.FindAsync(paymentIntent.TransactedById).ConfigureAwait(false);
			if (user != null)
			{
				transaction.TransactedById = paymentIntent.TransactedById;
				transaction.TransactedBy = user;
			}
			else
				error.AppendLine($"User {paymentIntent.TransactedById} does not exist.");

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
						var subscripton = new DbSubscription
						{
							Name = $"Susbscription for the plan {transaction.Plan.Name}",
							Owner = transaction.TransactedBy,
							PaymentTrasaction = transaction,
							StartedAt = DateTime.UtcNow,
							ExpireAt =  transaction.Plan.PlanType == PlanType.Monthly ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(365)
						};
						dbContext.Subscriptions.Add(subscripton);

						var workspace = new DbWorkspace
						{							
							Title = "New Workspace",
							Description = "A workspace to add boards, new members and assign roles.",
							Subscription = subscripton,							
							Owner = transaction.TransactedBy
						};
						dbContext.Workspaces.Add(workspace);

						transaction.Status = PaymentStatus.Processed;
					}

					await dbContext.SaveChangesAsync().ConfigureAwait(false);
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