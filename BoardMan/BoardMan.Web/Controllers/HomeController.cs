using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BoardMan.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ISubscriptionManager subscriptionManager;
		private readonly UserManager<AppUser> userManager;

		public HomeController(ILogger<HomeController> logger, ISubscriptionManager subscriptionManager, UserManager<AppUser> userManager)
		{
			_logger = logger;
			this.subscriptionManager = subscriptionManager;
			this.userManager = userManager;
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