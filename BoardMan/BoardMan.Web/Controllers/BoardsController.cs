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
	public class BoardsController : SiteControllerBase
	{
		private readonly IBoardManager boardManager;

		public BoardsController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger<BoardsController> logger, IStringLocalizer<SharedResource> sharedLocalizer, IBoardManager boardManager) : base(userManager, configuration, logger, sharedLocalizer)
		{
			this.boardManager = boardManager;
		}

		public IActionResult Index()
		{
			return View();
		}
		
		public async Task<IActionResult> Get(Guid boardId)
		{
			return View(await this.boardManager.GetBoardAsync(boardId));
		}

		public IActionResult Add(Guid workspaceId)
		{			
			return View(new Board { WorkspaceId = workspaceId});
		}

		[HttpPost]
		public async Task<IActionResult> Add(Board board)
		{
			await this.boardManager.CreateBoardAsync(board, this.userManager.GetGuidUserId(User));
			return this.RedirectToAction("Index", "Workspaces");
		}

		// Find how to do it with Delete
		//[HttpDelete]
		public async Task<IActionResult> Delete(Guid boardId)
		{
			await this.boardManager.DeleteBoardAsync(boardId);
			return this.RedirectToAction("Index", "Workspaces");
		}

		[HttpPost]
		public async Task<ActionResult> ListBoardMembers(Guid boardId, Guid? taskId)
		{
			return JsonResponse(ApiResponse.ListOptions(await this.boardManager.ListBoardMembersForDisplayAsync(boardId, this.userManager.GetGuidUserId(User))));
		}

		[HttpPost]
		public async Task<ActionResult> ListOtherLists(Guid boardId, Guid listId)
		{
			return JsonResponse(ApiResponse.ListOptions(await this.boardManager.ListOtherListsForDisplayAsync(boardId, listId)));
		}

		[HttpPost]
		public async Task<ActionResult> GetBoardMembers(Guid boardId)
		{
			return JsonResponse(ApiResponse.List(await this.boardManager.ListBoardMembersAsync(boardId, this.userManager.GetGuidUserId(User))));
		}

		[HttpPost]
		public async Task<ActionResult> CreateBoardMember(BoardMember boardMember)
		{
			if (ModelState.IsValid)
			{
				boardMember.AddedById = this.userManager.GetGuidUserId(User);
				var record = await this.boardManager.CreateBoardMemberAsync(boardMember, this.userManager.GetGuidUserId(User));
				return JsonResponse(ApiResponse.Single(record));
			}

			return JsonResponse(ApiResponse.Error(ModelState.Errors()));
		}


		[HttpPost]
		public async Task<ActionResult> UpdateBoardMember(BoardMember boardMember)
		{
			if (ModelState.IsValid)
			{				
				var record = await this.boardManager.EditBoardMemberAsync(boardMember, this.userManager.GetGuidUserId(User));
				return JsonResponse(ApiResponse.Single(record));
			}

			return JsonResponse(ApiResponse.Error(ModelState.Errors()));
		}

		[HttpPost]
		public async Task<ActionResult> DeleteBoardMember(Guid id)
		{
			if (ModelState.IsValid)
			{
				await this.boardManager.DeleteBoardMemberAsync(id);
				return JsonResponse(ApiResponse.Success());
			}

			return JsonResponse(ApiResponse.Error(ModelState.Errors()));
		}

		public async Task<ActionResult> ListProspectiveUsers(Guid boardId)
		{
			return Json(ApiResponse.List(await this.boardManager.ListProspectiveUsersAsync(this.userManager.GetGuidUserId(User), boardId)));
		}
	}
}
