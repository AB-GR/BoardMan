using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Models;
using Stripe;
using System.Text;

namespace BoardMan.Web.Managers
{
	public interface IPaymentService
	{
		Task<PaymentIntentResponseVM> CreatePaymentIntentAsync(PaymentIntentRequestVM request);

		Task<PaymentIntentVM> GetPaymentIntentAsync(PaymentSuccessRequestVM request);
	}

	public class PaymentService : IPaymentService
	{
		private readonly PaymentIntentService service;

		public PaymentService(PaymentIntentService service)
		{
			this.service = service;
		}

		public async Task<PaymentIntentResponseVM> CreatePaymentIntentAsync(PaymentIntentRequestVM request)
		{
			var options = new PaymentIntentCreateOptions
			{
				Amount = (long)request.Cost * 100, // stripe requires amount in fractions so no decimal places
				Currency = request.Currency.ToLowerInvariant(),
				Description = $"Sale of a {request.ProductName} {request.PlanName} ({request.PlanDescription}) plan",
				Metadata = new Dictionary<string, string>
						{
							{nameof(request.PlanId), request.PlanId.ToString()},
							{nameof(request.PlanName), request.PlanName},
							{nameof(PaymentIntentRequestVM.BillingDetails.UserEmail), request.BillingDetails.UserEmail},
							{nameof(PaymentIntentRequestVM.BillingDetails.UserFirstName), request.BillingDetails.UserFirstName},
							{nameof(PaymentIntentRequestVM.BillingDetails.UserLastName), request.BillingDetails.UserLastName},
							{nameof(PaymentIntentRequestVM.BillingDetails.NameAsOnCard), request.BillingDetails.NameAsOnCard}
						}
			};

			options.Metadata
				.AddIfNotNull(nameof(PaymentIntentRequestVM.BillingDetails.AddressLine1), request.BillingDetails.AddressLine1)
				.AddIfNotNull(nameof(PaymentIntentRequestVM.BillingDetails.AddressLine2), request.BillingDetails.AddressLine2)
				.AddIfNotNull(nameof(PaymentIntentRequestVM.BillingDetails.City), request.BillingDetails.City)
				.AddIfNotNull(nameof(PaymentIntentRequestVM.BillingDetails.State), request.BillingDetails.State)
				.AddIfNotNull(nameof(PaymentIntentRequestVM.BillingDetails.Country), request.BillingDetails.Country)
				.AddIfNotNull(nameof(PaymentIntentRequestVM.BillingDetails.ZipCode), request.BillingDetails.ZipCode)
				.AddIfNotNull(nameof(PaymentIntentRequestVM.UserId), request.UserId?.ToString());

			var paymentIntent = await this.service.CreateAsync(options);

			return new PaymentIntentResponseVM
			{
				PaymentIntentId = paymentIntent.Id,
				ClientSecret = paymentIntent.ClientSecret,
				ProductCode = $"{request.PlanName} ({request.PlanId})",
				BusinessName = request.BusinessName,
				ProductName = request.ProductName,
				Amount = paymentIntent.Amount,
				Currency = paymentIntent.Currency
			};
		}

		public async Task<PaymentIntentVM> GetPaymentIntentAsync(PaymentSuccessRequestVM request)
		{
			var paymentIntent = await this.service.GetAsync(request.PaymentIntentId,
				new PaymentIntentGetOptions { Expand = new List<string> { "payment_method" } }).ConfigureAwait(false);

			if (!"succeeded".Equals(paymentIntent?.Status, StringComparison.OrdinalIgnoreCase))
			{
				throw new PaymentException($"Payment failure received for intent Id {request.PaymentIntentId}");
			}

			var error = new StringBuilder();
			var paymentIntentVM = new PaymentIntentVM
			{
				PaymentReference = paymentIntent.Id,
				CostBeforeDiscount = (decimal)paymentIntent.Amount / 100,
				DiscountApplied = 0,
				FinalCost = (decimal)paymentIntent.Amount / 100,
				Currency = paymentIntent.Currency,
				Status = paymentIntent.Status.Equals("succeeded", StringComparison.InvariantCultureIgnoreCase) ? PaymentStatus.CanBeProcessed : PaymentStatus.Failed				
			};

			paymentIntentVM.StatusReason = Enum.GetName(typeof(PaymentStatus), paymentIntentVM.Status);

			if (paymentIntent.Metadata.TryParseValue(nameof(PaymentIntentRequestVM.PlanId), Guid.TryParse, out Guid planId))
			{
				paymentIntentVM.PlanId = planId;
			}
			else
			{
				error.AppendLine("Plan is missing.");
			}

			if (paymentIntent.Metadata.TryParseValue(nameof(PaymentIntentRequestVM.UserId), Guid.TryParse, out Guid userId))
			{
				paymentIntentVM.TransactedById = userId;
			}

			paymentIntentVM.BillingDetails = new BillingDetails();

			// Billing details 
			paymentIntentVM.BillingDetails.UserEmail = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.UserEmail), error);
			paymentIntentVM.BillingDetails.UserFirstName = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.UserFirstName), error);
			paymentIntentVM.BillingDetails.UserLastName = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.UserLastName), error);
			paymentIntentVM.BillingDetails.AddressLine1 = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.AddressLine1), error);
			paymentIntentVM.BillingDetails.AddressLine2 = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.AddressLine2), error);
			paymentIntentVM.BillingDetails.City = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.City), error);
			paymentIntentVM.BillingDetails.State = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.State), error);
			//paymentIntentVM.BillingDetails.Country = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.Country), error);
			paymentIntentVM.BillingDetails.ZipCode = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.ZipCode), error);
			paymentIntentVM.BillingDetails.NameAsOnCard = GetBillingDetailsProperty(paymentIntent, nameof(PaymentIntentRequestVM.BillingDetails.NameAsOnCard), error);

			paymentIntentVM.Errors = error.ToString();
			return paymentIntentVM;
		}

		private string GetBillingDetailsProperty(PaymentIntent paymentIntent, string key, StringBuilder error, bool isRequired = true)
		{
			string result = null;
			if (paymentIntent.Metadata.TryGetValue(key, out var parsedValue))
			{
				result = parsedValue;
			}
			else if(isRequired)
			{
				error.AppendLine($"{key} is missing.");
			}

			return result;
		}
	}
}
