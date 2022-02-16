using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BoardMan.Web.Controllers
{
	public class HomeController : SiteControllerBase
	{	
		private readonly ISubscriptionManager subscriptionManager;		

		public HomeController(ISubscriptionManager subscriptionManager, UserManager<AppUser> userManager, IConfiguration configuration, ILogger<HomeController> logger): base(userManager, configuration, logger)
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
	}
}