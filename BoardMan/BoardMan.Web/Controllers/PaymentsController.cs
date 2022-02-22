using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class PaymentsController : SiteControllerBase
	{
		private readonly IPlanManager planManager;
		private readonly IPaymentManager paymentManager;

		public PaymentsController(IPlanManager planManager, IPaymentManager paymentManager, UserManager<AppUser> userManager, IConfiguration configuration, ILogger<PaymentsController> logger, IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager, configuration, logger, sharedLocalizer)
		{
			this.planManager = planManager;
			this.paymentManager = paymentManager;
		}

		[HttpPost, AllowAnonymous]
		public async Task<ActionResult> CreatePaymentIntent([FromBody] PaymentIntentRequestVM request)
		{
			return await GetJsonAsync(async () =>
			{
				if (ModelState.IsValid)
				{
					var paymentIntentResponseVM = await this.paymentManager.CreatePaymentIntentAsync(request);
					return ApiResponse.Single(paymentIntentResponseVM);
				}

				return ApiResponse.Error(ModelState.Errors());
			});
		}

		[HttpPost, AllowAnonymous]
		public async Task<ActionResult> PaymentSuccess(PaymentSuccessRequestVM request)
		{
			if (!ModelState.IsValid)
				return RedirectWithMessage("Index",	"Home", "Missing payment identifier.");

			logger.LogInformation("Payment success received for intent Id {0}", request.PaymentIntentId);

			try
			{
				var paymentStatus = await this.paymentManager.ProcessPaymentAsync(request);
				if (paymentStatus == PaymentStatus.Processed)
				{
					return RedirectWithMessage("Index", "Home", "Payment success.");
				}
			}
			catch (PaymentException ex)
			{
				this.logger.LogError(ex, ex.Message);
				return RedirectWithMessage("Index", "Home", "Payment failed.");
			}

			return RedirectWithMessage("Index", "Home",
						$"Transaction failed. Don't worry we'll be looking into the issue shortly. You may also contact us at " + this.configuration["SupportMailId"]);
		}
	}
}
