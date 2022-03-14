using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class WorkspacesController : SiteControllerBase
	{
		private readonly IWorkspaceManager workspaceManager;

		public WorkspacesController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger<WorkspacesController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IWorkspaceManager workspaceManager) : base(userManager, configuration, logger, sharedLocalizer)
		{
			this.workspaceManager = workspaceManager;
		}

		public async Task<IActionResult> Index()
		{
			return View(await this.workspaceManager.GetAllWorkSpacesAsync(this.userManager.GetGuidUserId(User)));
		}
	}
}
