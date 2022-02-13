using BoardMan.Web.Managers;
using Microsoft.AspNetCore.Mvc;

namespace BoardMan.Web.Controllers
{
	public class PlansController : Controller
	{
		private readonly IPlanManager planManager;

		public PlansController(IPlanManager planManager)
		{
			this.planManager = planManager;
		}

		// GET: PlansController
		public async Task<ActionResult> Index()
		{
			var plans = await planManager.GetAllPlansAsync();
			return View(plans);
		}

		// GET: PlansController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: PlansController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: PlansController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: PlansController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: PlansController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: PlansController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: PlansController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
