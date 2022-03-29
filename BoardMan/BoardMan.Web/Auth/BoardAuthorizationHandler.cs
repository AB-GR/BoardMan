using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Auth
{
	public class BoardAuthorizationHandler : AuthorizationHandler<BoardAuthorizationrRequirement, Guid>
	{
		private readonly BoardManDbContext boardManDbContext;
		private readonly UserManager<AppUser> userManager;

		public BoardAuthorizationHandler(BoardManDbContext boardManDbContext, UserManager<AppUser> userManager) 
		{
			this.boardManDbContext = boardManDbContext;
			this.userManager = userManager;
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BoardAuthorizationrRequirement requirement, Guid boardId)
		{
			var currentUserId = this.userManager.GetGuidUserId(context.User);
			var board = await this.boardManDbContext.Boards.FirstOrDefaultAsync(x => x.Id == boardId && x.DeletedAt == null);
			if (board != null && board.OwnerId == currentUserId)
			{
				// Is Board SuperAdmin
				context.Succeed(requirement);
				return;
			}

			if (await this.boardManDbContext.BoardMembers.AnyAsync(x => x.BoardId == boardId && x.MemberId == currentUserId && requirement.RolesAllowed.Contains(x.Role.Name)))
			{
				context.Succeed(requirement);
				return;
			}

			return;
		}
	}

	public class BoardAuthorizationrRequirement : IAuthorizationRequirement 
	{
		public BoardAuthorizationrRequirement()
		{
			RolesAllowed = new List<string>();
		}

		public BoardAuthorizationrRequirement(List<string> rolesAllowed)
		{
			RolesAllowed = rolesAllowed;
		}

		public List<string> RolesAllowed { get; private set; } = null!;
	}
}
