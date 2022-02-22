using BoardMan.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class SubscriptionsController : SiteControllerBase
	{
		public SubscriptionsController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger<SubscriptionsController> logger, IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager, configuration, logger, sharedLocalizer)
		{
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
