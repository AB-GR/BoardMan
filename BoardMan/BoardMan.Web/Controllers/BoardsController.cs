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
	public class BoardsController : SiteControllerBase
	{
		private readonly IBoardManager boardManager;

		public BoardsController(UserManager<AppUser> userManager, IAuthorizationService authorizationService, IConfiguration configuration, ILogger<BoardsController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IBoardManager boardManager) : base(userManager, authorizationService, configuration, logger, sharedLocalizer)
		{
			this.boardManager = boardManager;
		}

		public IActionResult Index()
		{
			return View();
		}
		
		public async Task<IActionResult> Get(Guid boardId)
		{
			return await AuthorizedResposeAsync(async () =>
			{
				return View(await this.boardManager.GetBoardAsync(boardId));
			}, new EntityResource { Id = boardId, Type = EntityType.Board }, Policies.BoardReaderPolicy);		
		}

		public async Task<IActionResult> Add(Guid workspaceId)
		{
			return await AuthorizedResposeAsync(async () =>
			{
				return View(new Board { WorkspaceId = workspaceId });
			}, new EntityResource { Id = workspaceId, Type = EntityType.Workspace }, Policies.WorkspaceContributorWithBoardLimitPolicy);		
		}

		[HttpPost]
		public async Task<IActionResult> Add(Board board)
		{
			return await AuthorizedResposeAsync(async () =>
			{
				await this.boardManager.CreateBoardAsync(board, this.userManager.GetGuidUserId(User));
				return this.RedirectToAction("Index", "Workspaces");

			}, new EntityResource { Id = board.WorkspaceId, Type = EntityType.Workspace }, Policies.WorkspaceContributorWithBoardLimitPolicy);
		}

		// Find how to do it with Delete
		//[HttpDelete]
		public async Task<IActionResult> Delete(Guid boardId)
		{
			return await AuthorizedResposeAsync(async () =>
			{
				await this.boardManager.DeleteBoardAsync(boardId);
				return this.RedirectToAction("Index", "Workspaces");

			}, new EntityResource { Id = boardId, Type = EntityType.Board }, Policies.BoardSuperAdminPolicy);			
		}		

		[HttpPost]
		public async Task<ActionResult> ListAssignees(Guid boardId)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				return JsonResponse(ApiResponse.ListOptions(await this.boardManager.ListAssigneesForDisplayAsync(boardId, this.userManager.GetGuidUserId(User))));

			}, new EntityResource { Id = boardId, Type = EntityType.Board }, Policies.BoardContributorPolicy);
		}

		[HttpPost]
		public async Task<ActionResult> ListWatchers(Guid boardId)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				return JsonResponse(ApiResponse.ListOptions(await this.boardManager.ListWatchersForDisplayAsync(boardId, this.userManager.GetGuidUserId(User))));

			}, new EntityResource { Id = boardId, Type = EntityType.Board }, Policies.BoardContributorPolicy);
		}

		[HttpPost]
		public async Task<ActionResult> ListOtherLists(Guid boardId, Guid listId)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				return JsonResponse(ApiResponse.ListOptions(await this.boardManager.ListOtherListsForDisplayAsync(boardId, listId)));

			}, new EntityResource { Id = boardId, Type = EntityType.Board }, Policies.BoardContributorPolicy);			
		}

		[HttpPost]
		public async Task<ActionResult> GetBoardMembers(Guid boardId)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				return JsonResponse(ApiResponse.List(await this.boardManager.ListBoardMembersAsync(boardId, this.userManager.GetGuidUserId(User))));

			}, new EntityResource { Id = boardId, Type = EntityType.Board }, Policies.BoardAdminPolicy);			
		}

		[HttpPost]
		public async Task<ActionResult> CreateBoardMember(BoardMember boardMember)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				if (ModelState.IsValid)
				{
					boardMember.AddedById = this.userManager.GetGuidUserId(User);
					var record = await this.boardManager.CreateBoardMemberAsync(boardMember, this.userManager.GetGuidUserId(User));
					return JsonResponse(ApiResponse.Single(record));
				}

				return JsonResponse(ApiResponse.Error(ModelState.Errors()));

			}, new EntityResource { Id = boardMember.BoardId.GetValueOrDefault(), Type = EntityType.Board }, Policies.BoardAdminPolicy);
			
		}

		[HttpPost]
		public async Task<ActionResult> UpdateBoardMember(BoardMember boardMember)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				if (ModelState.IsValid)
				{
					var record = await this.boardManager.EditBoardMemberAsync(boardMember, this.userManager.GetGuidUserId(User));
					return JsonResponse(ApiResponse.Single(record));
				}

				return JsonResponse(ApiResponse.Error(ModelState.Errors()));

			}, new EntityResource { Id = boardMember.BoardId.GetValueOrDefault(), Type = EntityType.Board }, Policies.BoardAdminPolicy);
		}

		[HttpPost]
		public async Task<ActionResult> DeleteBoardMember(Guid id)
		{
			return await AuthorizedJsonResposeAsync(async () =>
			{
				if (ModelState.IsValid)
				{
					await this.boardManager.DeleteBoardMemberAsync(id);
					return JsonResponse(ApiResponse.Success());
				}

				return JsonResponse(ApiResponse.Error(ModelState.Errors()));

			}, new EntityResource { Id = id, Type = EntityType.BoardMember }, Policies.BoardAdminPolicy);			
		}

		public async Task<ActionResult> ListProspectiveUsers(Guid boardId)
		{
			return await AuthorizedJsonAsync(async () =>
			{
				return Json(ApiResponse.List(await this.boardManager.ListProspectiveUsersAsync(this.userManager.GetGuidUserId(User), boardId)));

			}, new EntityResource { Id = boardId, Type = EntityType.Board }, Policies.BoardAdminPolicy);			
		}
	}
}
