using BoardMan.Web.Auth;
using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class WorkspacesController : SiteControllerBase
	{
		private readonly IWorkspaceManager workspaceManager;

		public WorkspacesController(UserManager<AppUser> userManager, IAuthorizationService authorizationService, IConfiguration configuration, ILogger<WorkspacesController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IWorkspaceManager workspaceManager) : base(userManager, authorizationService, configuration, logger, sharedLocalizer)
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
			return await AuthorizedJsonResposeAsync(async () =>
			{
				return JsonResponse(ApiResponse.List(await this.workspaceManager.ListWorkspaceMembersAsync(workspaceId, this.userManager.GetGuidUserId(User))));
			}, new EntityResource { Id = workspaceId, Type = EntityType.Workspace }, Policies.WorkspaceAdminPolicy);			
		}

		[HttpPost]
		public async Task<ActionResult> CreateWorkspaceMember(WorkspaceMember workspaceMember)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				if (ModelState.IsValid)
				{
					workspaceMember.AddedById = this.userManager.GetGuidUserId(User);
					var record = await this.workspaceManager.CreateWorkspaceMemberAsync(workspaceMember, this.userManager.GetGuidUserId(User));
					return JsonResponse(ApiResponse.Single(record));
				}

				return JsonResponse(ApiResponse.Error(ModelState.Errors()));
			}, new EntityResource { Id = workspaceMember.WorkspaceId.GetValueOrDefault(), Type = EntityType.Workspace }, Policies.WorkspaceAdminPolicy);			
		}


		[HttpPost]
		public async Task<ActionResult> UpdateWorkspaceMember(WorkspaceMember workspaceMember)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				if (ModelState.IsValid)
				{
					var record = await this.workspaceManager.EditWorkspaceMemberAsync(workspaceMember, this.userManager.GetGuidUserId(User));
					return JsonResponse(ApiResponse.Single(record));
				}

				return JsonResponse(ApiResponse.Error(ModelState.Errors()));
			}, new EntityResource { Id = workspaceMember.WorkspaceId.GetValueOrDefault(), Type = EntityType.Workspace }, Policies.WorkspaceAdminPolicy);			
		}

		[HttpPost]
		public async Task<ActionResult> DeleteWorkspaceMember(Guid id)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				if (ModelState.IsValid)
				{
					await this.workspaceManager.DeleteWorkspaceMemberAsync(id);
					return JsonResponse(ApiResponse.Success());
				}

				return JsonResponse(ApiResponse.Error(ModelState.Errors()));
			}, new EntityResource { Id = id, Type = EntityType.WorkspaceMember }, Policies.WorkspaceAdminPolicy);			
		}

		public async Task<ActionResult> ListProspectiveUsers(Guid workspaceId)
		{
			return await AuthorizedJsonAsync(async () =>
			{
				return Json(ApiResponse.List(await this.workspaceManager.ListProspectiveUsersAsync(this.userManager.GetGuidUserId(User), workspaceId)));

			}, new EntityResource { Id = workspaceId, Type = EntityType.Workspace }, Policies.WorkspaceAdminPolicy);			
		}
	}
}
