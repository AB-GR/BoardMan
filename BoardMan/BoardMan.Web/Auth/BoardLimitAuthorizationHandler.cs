using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Auth
{
	public class BoardLimitAuthorizationHandler : AuthorizationHandler<BoardLimitAuthorizatioRequirement, EntityResource>
	{
		private readonly BoardManDbContext boardManDbContext;
		private readonly UserManager<AppUser> userManager;

		public BoardLimitAuthorizationHandler(BoardManDbContext boardManDbContext, UserManager<AppUser> userManager)
		{
			this.boardManDbContext = boardManDbContext;
			this.userManager = userManager;
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BoardLimitAuthorizatioRequirement requirement, EntityResource resource)
		{
			var currentUserId = this.userManager.GetGuidUserId(context.User);
			var workspaceId = await GetWorkspaceIdAsync(resource);
			var workspace = await this.boardManDbContext.Workspaces.Include(x => x.Boards).FirstOrDefaultAsync(x => x.Id == workspaceId && x.DeletedAt == null);
			if (workspace == null)
			{
				return;
			}

			var latestSubscription = await this.boardManDbContext.Subscriptions
				.Include(x => x.PaymentTrasaction.Plan)
				.Where(x => x.OwnerId == workspace.OwnerId && x.DeletedAt == null)
				.OrderByDescending(x => x.ExpireAt).FirstOrDefaultAsync();

			if (latestSubscription == null || latestSubscription.ExpireAt < DateTime.UtcNow)
			{
				return;
			}

			// If Application Super Admin
			if (context.User.IsInRole(Roles.ApplicationSuperAdmin))
			{
				context.Succeed(requirement);
				return;
			}

			if (latestSubscription.PaymentTrasaction.Plan.BoardLimit == null || latestSubscription.PaymentTrasaction.Plan.BoardLimit > workspace.Boards.Count(x => x.DeletedAt == null))
			{
				context.Succeed(requirement);
				return;
			}
		}

		private async Task<Guid> GetWorkspaceIdAsync(EntityResource resource)
		{
			switch (resource.Type)
			{
				case EntityType.Workspace:
					return resource.Id;
				case EntityType.WorkspaceMember:
					var dbWorkspaceMember = await this.boardManDbContext.WorkspaceMembers.FirstOrDefaultAsync(x => x.Id == resource.Id);
					return dbWorkspaceMember?.WorkspaceId ?? Guid.Empty;
				default:
					return Guid.Empty;
			}
		}
	}

	public class BoardLimitAuthorizatioRequirement : IAuthorizationRequirement
	{
	}
}
