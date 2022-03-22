using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using System.Text;
using System.Text.Encodings.Web;

namespace BoardMan.Web.Controllers
{
	public class EmailInvitesController : SiteControllerBase
	{
		private readonly IEmailInviteManager emailInviteManager;
		private readonly IWorkspaceManager workspaceManager;
		private readonly SignInManager<AppUser> signInManager;
		private readonly IEmailSender emailSender;

		public EmailInvitesController(SignInManager<AppUser> signInManager, IEmailSender emailSender, IWorkspaceManager workspaceManager, UserManager<AppUser> userManager, IConfiguration configuration, ILogger<EmailInvitesController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IEmailInviteManager emailInviteManager) : base(userManager, configuration, logger, sharedLocalizer)
		{
			this.signInManager = signInManager;
			this.emailSender = emailSender;
			this.emailInviteManager = emailInviteManager;
			this.workspaceManager = workspaceManager;
		}

		public async Task<IActionResult> Index(string tokenId)
		{
			return View(await this.emailInviteManager.ValidateToken(tokenId));
		}

		[HttpPost]
		public async Task<IActionResult> Index(EmailInviteModel model)
		{
			if (ModelState.IsValid)
			{
				var user = CreateUser();
				user.FirstName = model.FirstName;
				user.LastName = model.LastName;
				user.UserName = model.Email;
				user.Email = model.Email;
				var result = await userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					logger.LogInformation("User created a new account with password.");

					var userId = await userManager.GetUserIdAsync(user);
					
					// Create a workspace
					await workspaceManager.CreateOrUpdateWorskpaceAsync(user.Id);

					// Create a member
					await emailInviteManager.CreateMember(model.Token, user.Id);
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
						return RedirectWithMessage(registerConfirmUrl, "Email invite success. Please confirm your email and then login and check your subscription");
					}
					else
					{
						await signInManager.SignInAsync(user, isPersistent: false);
						return RedirectWithMessage("Index", "Home", "Your user account has been set up.");
					}
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			
			return View(model);
		}

		private AppUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<AppUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
					$"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}
	}
}
