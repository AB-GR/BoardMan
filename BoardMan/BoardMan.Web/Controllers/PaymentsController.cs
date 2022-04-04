using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using System.Text;
using System.Text.Encodings.Web;

namespace BoardMan.Web.Controllers
{
	[AllowAnonymous]
	public class PaymentsController : SiteControllerBase
	{		
		private readonly IPaymentManager paymentManager;
		private readonly SignInManager<AppUser> signInManager;
		private readonly IEmailSender emailSender;

		public PaymentsController(IPaymentManager paymentManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, UserManager<AppUser> userManager, IAuthorizationService authorizationService, IConfiguration configuration, ILogger<PaymentsController> logger, IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager, authorizationService, configuration, logger, sharedLocalizer)
		{			
			this.paymentManager = paymentManager;
			this.signInManager = signInManager;
			this.emailSender = emailSender;	
		}

		[HttpPost]
		public async Task<ActionResult> ValidatePayment([FromBody] ValidatePaymentRequest request)
		{
			return await GetJsonAsync(async () =>
			{
				if (ModelState.IsValid)
				{
					var validationResult = await this.paymentManager.ValidatePaymentAsync(request);
					return ApiResponse.Single(validationResult);
				}

				return ApiResponse.Error(ModelState.Errors());
			});
		}

		[HttpPost]
		public async Task<ActionResult> CreatePaymentIntent([FromBody] PaymentIntentRequest request)
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

		[HttpPost]
		public async Task<ActionResult> PaymentSuccess(PaymentSuccessRequest request)
		{
			if (!ModelState.IsValid)
				return RedirectWithMessage("Index",	"Home", "Missing payment identifier.");

			logger.LogInformation("Payment success received for intent Id {0}", request.PaymentIntentId);

			try
			{
				var paymentResult = await this.paymentManager.ProcessPaymentAsync(request);
				if (paymentResult.PaymentStatus == PaymentStatus.Processed)
				{
					if(paymentResult.UserDetails.UserCreated)
					{
						logger.LogInformation("User created a new account with password.");

						var user = paymentResult.UserDetails.User;
						var userId = await userManager.GetUserIdAsync(user);
						var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
						code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
						var callbackUrl = Url.Page(
							"/Account/ConfirmEmail",
							pageHandler: null,
							values: new { area = "Identity", userId = userId, code = code },
							protocol: Request.Scheme);

						await emailSender.SendEmailAsync(user.Email, "Confirm your email",
							$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

						if (userManager.Options.SignIn.RequireConfirmedAccount)
						{							
							var registerConfirmUrl = Url.Page(
							"/Account/RegisterConfirmation",
							pageHandler: null,
							values: new { area = "Identity", email = user.Email },
							protocol: Request.Scheme);							
							return RedirectWithMessage(registerConfirmUrl, "Payment success. Please confirm your email and then login and check your subscription");
						}
						else
						{
							await signInManager.SignInAsync(user, isPersistent: false);
							return RedirectWithMessage("Index", "Home", "Payment success.");
						}
					}

					return RedirectWithMessage("Index", "Home", $"Payment success. {(paymentResult.UserDetails.UserIsLoggedIn ? "new subscription has been created" : "Login and verify the new subscription" )}");
				}

				// ask to login if done anonymously
			}
			catch (PaymentException ex)
			{
				this.logger.LogError(ex, ex.Message);
				return RedirectWithMessage("Index", "Home", "Payment failed. Don't worry we'll be looking into the issue shortly. You may also contact us at " + this.configuration["SupportMailId"]);
			}

			return RedirectWithMessage("Index", "Home",
						$"Payment failed. Don't worry we'll be looking into the issue shortly. You may also contact us at " + this.configuration["SupportMailId"]);
		}
	}
}
