using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class SubscriptionsController : SiteControllerBase
	{
		public SubscriptionsController(ISubscriptionManager subscriptionManager, UserManager<AppUser> userManager, IConfiguration configuration, ILogger<SubscriptionsController> logger, IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager, configuration, logger, sharedLocalizer)
		{
			this.subscriptionManager = subscriptionManager;
		}

		private ISubscriptionManager subscriptionManager;

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetSubscriptions()
		{
			return JsonResponse(ApiResponse.List(await this.subscriptionManager.GetSubscriptionsAsync(this.userManager.GetGuidUserId(User))));			
		}
	}
}
