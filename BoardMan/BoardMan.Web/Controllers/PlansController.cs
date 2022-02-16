using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using BoardMan.Web.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BoardMan.Web.Data;

namespace BoardMan.Web.Controllers
{
	public class PlansController : SiteControllerBase
	{
		private readonly IPlanManager planManager;		

		public PlansController(IPlanManager planManager, UserManager<AppUser> userManager, IConfiguration configuration, ILogger<PlansController> logger): base(userManager, configuration, logger)
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

		// GET: PlansController/Buy
		public async Task<ActionResult> Buy(Guid planId)
		{
			var plan = await this.planManager.GetPlanAsync(planId);
			if (plan == null)
			{
				this.logger.LogInformation("No plan exists with the planId in /Plans/Buy?{planId}", planId);
				return new NotFoundViewResult("PlanNotFound");
			}

			var stripeApiKey = configuration.GetValue<string>("StripePublicKey");
			if(string.IsNullOrWhiteSpace(stripeApiKey))
			{
				this.logger.LogError("StripePublicKey has not been configured and is required for payment integration");
				throw new InsufficientDataToProcessException("Payment integration could not be initialized");
			}

			var currentUser = await this.userManager.GetUserAsync(User);
			return View(new BuyPlanVM
			{
				PlanId = planId,
				PlanDescription = plan.Description,
				PlanName = plan.Name,
				Cost = plan.Cost,
				Currency = plan.Currency,
				PaymentKey = stripeApiKey,				 
				UserId = currentUser.Id,
				BillingDetails = new BillingDetails
				{
					UserEmail = currentUser.Email,
					UserFirstName = currentUser.FirstName,
					UserLastName = currentUser.LastName
				}
			});
		}
	}
}
