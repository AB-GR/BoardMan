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
		Task<PaymentIntentResponse> CreatePaymentIntentAsync(PaymentIntentRequest request);

		Task<PaymentIntentTransaction> GetPaymentIntentAsync(PaymentSuccessRequest request);
	}

	public class PaymentService : IPaymentService
	{
		private readonly PaymentIntentService service;

		public PaymentService(PaymentIntentService service)
		{
			this.service = service;
		}

		public async Task<PaymentIntentResponse> CreatePaymentIntentAsync(PaymentIntentRequest request)
		{
			var options = new PaymentIntentCreateOptions
			{
				Amount = (long)request.Cost * 100, // stripe requires amount in fractions so no decimal places
				Currency = request.Currency.ToLowerInvariant(),
				Description = $"Sale of a {request.ProductName} {request.PlanName} ({request.PlanDescription}) plan",
				Metadata = new Dictionary<string, string>
				{
					{nameof(request.PlanId), request.PlanId.ToString()},
					{nameof(request.PlanName), request.PlanName}
				}
			};

			var paymentIntent = await this.service.CreateAsync(options);

			return new PaymentIntentResponse
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

		public async Task<PaymentIntentTransaction> GetPaymentIntentAsync(PaymentSuccessRequest request)
		{
			var paymentIntent = await this.service.GetAsync(request.PaymentIntentId,
				new PaymentIntentGetOptions { Expand = new List<string> { "payment_method" } }).ConfigureAwait(false);

			if (!"succeeded".Equals(paymentIntent?.Status, StringComparison.OrdinalIgnoreCase))
			{
				throw new PaymentException($"Payment failure received for intent Id {request.PaymentIntentId}");
			}

			var error = new StringBuilder();
			var paymentIntentVM = new PaymentIntentTransaction
			{
				PaymentReference = paymentIntent.Id,
				CostBeforeDiscount = (decimal)paymentIntent.Amount / 100,
				DiscountApplied = 0,
				FinalCost = (decimal)paymentIntent.Amount / 100,
				Currency = paymentIntent.Currency,
				Status = paymentIntent.Status.Equals("succeeded", StringComparison.InvariantCultureIgnoreCase) ? PaymentStatus.CanBeProcessed : PaymentStatus.Failed,
				RawData = paymentIntent.StripeResponse.Content
			};

			paymentIntentVM.StatusReason = Enum.GetName(typeof(PaymentStatus), paymentIntentVM.Status);

			if (paymentIntent.Metadata.TryParseValue(nameof(PaymentIntentRequest.PlanId), Guid.TryParse, out Guid planId))
			{
				paymentIntentVM.PlanId = planId;
			}
			else
			{
				error.AppendLine("Plan is missing.");
			}
				

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
