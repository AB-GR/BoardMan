using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace BoardMan.Web.Controllers
{
	[AllowAnonymous]
	public class HomeController : SiteControllerBase
	{	
		private readonly ISubscriptionManager subscriptionManager;		

		public HomeController(ISubscriptionManager subscriptionManager, UserManager<AppUser> userManager, IConfiguration configuration, ILogger<HomeController> logger, IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager, configuration, logger, sharedLocalizer)
		{			
			this.subscriptionManager = subscriptionManager;			
		}

		public async Task<IActionResult> IndexAsync()
		{			
			return View(await subscriptionManager.GetSubscriptionNotificationAsync(this.userManager.GetGuidUserId(User)));
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[HttpPost]
		public IActionResult SetLanguage(string culture, string returnUrl)
		{
			Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			);

			return LocalRedirect(returnUrl);
		}
	}
}