using BoardMan.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BoardMan.Web.Controllers
{
	public class PaymentsController : SiteControllerBase
	{
		public PaymentsController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger logger) : base(userManager, configuration, logger)
		{
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
