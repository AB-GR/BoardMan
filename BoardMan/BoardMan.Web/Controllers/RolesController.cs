using BoardMan.Web.Data;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class RolesController : SiteControllerBase
	{
		private readonly IRoleManager roleManager;

		public RolesController(UserManager<DbAppUser> userManager, IAuthorizationService authorizationService, IConfiguration configuration, ILogger<RolesController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IRoleManager roleManager = null) : base(userManager, authorizationService, configuration, logger, sharedLocalizer)
		{
			this.roleManager = roleManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> ListRoles(RoleType roleType)
		{
			return JsonResponse(ApiResponse.ListOptions(await this.roleManager.ListRolesAsync(roleType)));
		}
	}
}
