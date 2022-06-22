using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using BoardMan.Web.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BoardMan.Web.Data;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;

namespace BoardMan.Web.Controllers
{
	public class PlansController : SiteControllerBase
	{
		private readonly IPlanManager planManager;		

		public PlansController(IPlanManager planManager, UserManager<DbAppUser> userManager, IAuthorizationService authorizationService, IConfiguration configuration, ILogger<PlansController> logger, IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager, authorizationService, configuration, logger, sharedLocalizer)
		{ 
			this.planManager = planManager;			
		}
		
		[AllowAnonymous]
		public async Task<ActionResult> Index()
		{
			var plans = await planManager.GetAllPlansAsync();
			return View(plans);
		}
		
		[AllowAnonymous]
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
			return View(new BuyPlan
			{
				PlanId = planId,
				PlanDescription = plan.Description,
				PlanName = plan.Name,
				Cost = plan.Cost,
				Currency = plan.Currency,
				PaymentKey = stripeApiKey,				 
				UserId = currentUser?.Id,
				BillingDetails = new BillingDetails
				{
					UserEmail = currentUser?.Email,
					UserFirstName = currentUser?.FirstName,
					UserLastName = currentUser?.LastName,
					IsAnonymousUser = currentUser == null
				}
			});
		}
	}
}
