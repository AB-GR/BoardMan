using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Stripe;

namespace BoardMan.Web.Controllers
{
	public class PaymentsController : SiteControllerBase
	{
		private readonly IPlanManager planManager;

		public PaymentsController(IPlanManager planManager, UserManager<AppUser> userManager, IConfiguration configuration, ILogger logger, IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager, configuration, logger, sharedLocalizer)
		{
			this.planManager = planManager;
		}

		[HttpPost, AllowAnonymous]
		public async Task<ActionResult> CreatePaymentIntent(PaymentIntentRequestVM request)
		{
			return await GetJsonAsync(async () =>
			{
				if (ModelState.IsValid)
				{
					var service = new PaymentIntentService();

					var productName = "Boardman";
					var businessName = "ABTech";
					var plan = await planManager.GetPlanAsync(request.PlanId);

					if (plan == null)
						return ApiResponse.Error($"Plan {request.PlanId} does not exist."); 

					var options = new PaymentIntentCreateOptions
					{
						Amount = (long)plan.Cost * 100, // stripe requires amount in fractions so no decimal places
						Currency = request.Currency.ToLowerInvariant(),
						Description = $"Sale of a {productName} {plan.Name} ({plan.Description}) plan",
						Metadata = new Dictionary<string, string>
						{
							{nameof(plan.Id), plan.Id.ToString()},
							{nameof(plan.Name), plan.Name},
							{nameof(PaymentIntentRequestVM.BillingDetails.UserEmail), request.BillingDetails.UserEmail},
							{nameof(PaymentIntentRequestVM.BillingDetails.UserFirstName), request.BillingDetails.UserFirstName},
							{nameof(PaymentIntentRequestVM.BillingDetails.UserLastName), request.BillingDetails.UserLastName}
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

					var paymentIntent = await service.CreateAsync(options);

					return ApiResponse.Single(new PaymentIntentResponseVM
					{
						PaymentIntentId = paymentIntent.Id,
						ClientSecret = paymentIntent.ClientSecret,
						ProductCode = $"{plan.Name} ({plan.Id})",
						BusinessName = businessName,
						ProductName = productName,
						Amount = paymentIntent.Amount,
						Currency = paymentIntent.Currency
					});
				}

				return ApiResponse.Error(ModelState.Errors());
			});
		}

		[HttpPost, AllowAnonymous]
		public ActionResult PaymentSuccess(PaymentSuccessRequestVM request)
		{
			if (!ModelState.IsValid)
				return RedirectWithMessage("Index",	"Home", "Missing payment identifier.");

			logger.LogInformation("Payment success received for intent Id {0}", request.PaymentIntentId);

			var service = new PaymentIntentService();
			var paymentIntent = service.Get(request.PaymentIntentId,
				new PaymentIntentGetOptions { Expand = new List<string> { "payment_method" } });

			// todo: log the intent even if it has failed.
			if (!"succeeded".Equals(paymentIntent?.Status, StringComparison.OrdinalIgnoreCase))
				return RedirectWithMessage("Index",	"Home", "Payment failed.");

			return RedirectWithMessage("Index", "Home", "Payment success.");
			//var result = ProcessPayment(paymentIntent);
			//if (result.Succeeded)
			//{
			//	if (!string.IsNullOrWhiteSpace(result.Token))
			//	{
			//		return RedirectWithMessage("Reset", "Account", new { id = result.Token },
			//			"Your purchase succeeded! Thank you. Please set your account password to login. The email is the same you provided during purchase.");
			//	}

			//	return RedirectWithMessage("Index", "Home", "Your purchase succeeded! Thank you.");
			//}
			//else
			//{
			//	if (!string.IsNullOrWhiteSpace(result.Token))
			//	{
			//		return RedirectWithMessage("Reset", "Account", new { id = result.Token },
			//			"Thank you. There's some issue with the transaction and we are looking into that. Meanwhile we've setup your account. Please set the password to login. The email is the same you provided during purchase.");
			//	}
			//	else
			//		return RedirectWithMessage("Index", "Home",
			//			$"Transaction failed. Don't worry we'll be looking into the issue shortly. You may also contact us at " + configuration["SupportMailId"]);
			//}
		}
	}
}
