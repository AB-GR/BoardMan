using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Auth
{
	public class WorkspaceAuthorizationHandler : AuthorizationHandler<WorkspaceAuthorizationrRequirement, EntityResource>
	{
		private readonly BoardManDbContext boardManDbContext;
		private readonly UserManager<DbAppUser> userManager;

		public WorkspaceAuthorizationHandler(BoardManDbContext boardManDbContext, UserManager<DbAppUser> userManager)
		{
			this.boardManDbContext = boardManDbContext;
			this.userManager = userManager;
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkspaceAuthorizationrRequirement requirement, EntityResource resource)
		{
			var currentUserId = this.userManager.GetGuidUserId(context.User);
			var workspaceId = await GetWorkspaceIdAsync(resource);
			var workspace = await this.boardManDbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == workspaceId && x.DeletedAt == null);
			if(workspace == null)
			{
				return;
			}

			var latestSubscription = await this.boardManDbContext.Subscriptions
				.Where(x => x.OwnerId == workspace.OwnerId && x.DeletedAt == null)
				.OrderByDescending(x => x.ExpireAt).FirstOrDefaultAsync();
			if(latestSubscription == null  || latestSubscription.ExpireAt < DateTime.UtcNow)
			{
				return;
			}

			// If Application Super Admin
			if(context.User.IsInRole(Roles.ApplicationSuperAdmin))
			{
				context.Succeed(requirement);
				return;
			}

			// Is Workspace SuperAdmin
			if (workspace.OwnerId == currentUserId)
			{	
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

		private async Task<Guid> GetWorkspaceIdAsync(EntityResource resource)
		{
			switch (resource.Type)
			{
				case EntityType.Workspace:
					return resource.Id;
				case EntityType.WorkspaceMember:
					var dbWorkspaceMember = await this.boardManDbContext.WorkspaceMembers.FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
					if (dbWorkspaceMember != null)
						return dbWorkspaceMember.Id;
					else
					{
						var dbEmailInvite = await this.boardManDbContext.EmailInvites.FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
						return dbEmailInvite != null ? dbEmailInvite.EntityUrn.ToEntityUrn().EntityId : Guid.Empty;
					}
				default:
					return Guid.Empty;
			}
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
