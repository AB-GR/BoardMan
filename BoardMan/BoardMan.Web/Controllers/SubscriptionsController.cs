using Microsoft.AspNetCore.Mvc;

namespace BoardMan.Web.Controllers
{
	public class SubscriptionsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
