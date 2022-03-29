using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Auth
{
	public class WorkspaceAuthorizationHandler : AuthorizationHandler<WorkspaceAuthorizationrRequirement, Guid>
	{
		private readonly BoardManDbContext boardManDbContext;
		private readonly UserManager<AppUser> userManager;

		public WorkspaceAuthorizationHandler(BoardManDbContext boardManDbContext, UserManager<AppUser> userManager)
		{
			this.boardManDbContext = boardManDbContext;
			this.userManager = userManager;
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkspaceAuthorizationrRequirement requirement, Guid workspaceId)
		{
			var currentUserId = this.userManager.GetGuidUserId(context.User);
			var board = await this.boardManDbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == workspaceId && x.DeletedAt == null);
			if (board != null && board.OwnerId == currentUserId)
			{
				// Is Board SuperAdmin
				context.Succeed(requirement);
				return;
			}

			if (await this.boardManDbContext.WorkspaceMembers.AnyAsync(x => x.WorkspaceId == workspaceId && x.MemberId == currentUserId && requirement.RolesAllowed.Contains(x.Role.Name)))
			{
				context.Succeed(requirement);
				return;
			}

			return;
		}
	}

	public class WorkspaceAuthorizationrRequirement : IAuthorizationRequirement
	{
		public WorkspaceAuthorizationrRequirement()
		{
			RolesAllowed = new List<string>();
		}

		public WorkspaceAuthorizationrRequirement(List<string> rolesAllowed)
		{
			RolesAllowed = rolesAllowed;
		}

		public List<string> RolesAllowed { get; private set; } = null!;
	}
}
