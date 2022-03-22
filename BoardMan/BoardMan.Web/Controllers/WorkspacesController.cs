using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
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

		[HttpPost]
		public async Task<ActionResult> GetWorkspaceMembers(Guid workspaceId)
		{
			return JsonResponse(ApiResponse.List(await this.workspaceManager.ListWorkspaceMembersAsync(workspaceId, this.userManager.GetGuidUserId(User))));
		}

		[HttpPost]
		public async Task<ActionResult> CreateWorkspaceMember(WorkspaceMember workspaceMember)
		{
			if (ModelState.IsValid)
			{
				workspaceMember.AddedById = this.userManager.GetGuidUserId(User);
				var record = await this.workspaceManager.CreateWorkspaceMemberAsync(workspaceMember, this.userManager.GetGuidUserId(User));
				return JsonResponse(ApiResponse.Single(record));
			}

			return JsonResponse(ApiResponse.Error(ModelState.Errors()));
		}


		[HttpPost]
		public async Task<ActionResult> UpdateWorkspaceMember(WorkspaceMember workspaceMember)
		{
			if (ModelState.IsValid)
			{
				var record = await this.workspaceManager.EditWorkspaceMemberAsync(workspaceMember, this.userManager.GetGuidUserId(User));
				return JsonResponse(ApiResponse.Single(record));
			}

			return JsonResponse(ApiResponse.Error(ModelState.Errors()));
		}

		[HttpPost]
		public async Task<ActionResult> DeleteWorkspaceMember(Guid id)
		{
			if (ModelState.IsValid)
			{
				await this.workspaceManager.DeleteWorkspaceMemberAsync(id);
				return JsonResponse(ApiResponse.Success());
			}

			return JsonResponse(ApiResponse.Error(ModelState.Errors()));
		}

		public async Task<ActionResult> ListProspectiveUsers(Guid workspaceId)
		{
			return Json(ApiResponse.List(await this.workspaceManager.ListProspectiveUsersAsync(this.userManager.GetGuidUserId(User), workspaceId)));
		}
	}
}
