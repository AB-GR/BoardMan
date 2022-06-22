using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Auth
{
	public class BoardAuthorizationHandler : AuthorizationHandler<BoardAuthorizationrRequirement, EntityResource>
	{
		private readonly BoardManDbContext boardManDbContext;
		private readonly UserManager<DbAppUser> userManager;

		public BoardAuthorizationHandler(BoardManDbContext boardManDbContext, UserManager<DbAppUser> userManager) 
		{
			this.boardManDbContext = boardManDbContext;
			this.userManager = userManager;
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BoardAuthorizationrRequirement requirement, EntityResource resource)
		{
			var currentUserId = this.userManager.GetGuidUserId(context.User);
			var boardId = await GetBoardIdAsync(resource);
			var board = await this.boardManDbContext.Boards.Include(x => x.Workspace).FirstOrDefaultAsync(x => x.Id == boardId && x.DeletedAt == null);
			if (board == null)
			{
				return;
			}

			var latestSubscription = await this.boardManDbContext.Subscriptions
				.Where(x => x.OwnerId == board.Workspace.OwnerId && x.DeletedAt == null)
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

			// Is Board SuperAdmin or Workspace SuperAdmin
			if (board.OwnerId == currentUserId || board.Workspace.OwnerId  == currentUserId)
			{				
				context.Succeed(requirement);
				return;
			}

			if (await this.boardManDbContext.BoardMembers.AnyAsync(x => x.BoardId == boardId && x.MemberId == currentUserId && requirement.RolesAllowed.Contains(x.Role.Name)))
			{
				if(resource.Type == EntityType.TaskComment && (await this.boardManDbContext.TaskComments.FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null))?.CommentedById != currentUserId)
				{
					return;
				}

				if (resource.Type == EntityType.TaskAttachment && resource.Action == UserAction.Delete && (await this.boardManDbContext.TaskAttachments.FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null))?.UploadedById != currentUserId)
				{
					return;
				}

				context.Succeed(requirement);
				return;
			}

			return;
		}

        private async Task<Guid> GetBoardIdAsync(EntityResource resource)
        {
            switch (resource.Type)
            {
				case EntityType.Board:
					return resource.Id;
				case EntityType.BoardMember:					
					var dbBoardMember = await this.boardManDbContext.BoardMembers.FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
					if(dbBoardMember != null)
						return dbBoardMember.Id;
					else
					{
						var dbEmailInvite = await this.boardManDbContext.EmailInvites.FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
						return dbEmailInvite != null ? dbEmailInvite.EntityUrn.ToEntityUrn().EntityId : Guid.Empty;
					}
				case EntityType.List:
					var dbList = await this.boardManDbContext.Lists.FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
					return dbList?.BoardId ?? Guid.Empty;
				case EntityType.Task:
					var dbTask = await this.boardManDbContext.Tasks.Include(x => x.List).FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
					return dbTask?.List.BoardId ?? Guid.Empty;
				case EntityType.TaskComment:
					var dbTaskComment = await this.boardManDbContext.TaskComments.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
					return dbTaskComment?.Task.List.BoardId ?? Guid.Empty;
				case EntityType.Tasklabel:
					var dbTaskLabel = await this.boardManDbContext.TaskLabels.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
					return dbTaskLabel?.Task.List.BoardId ?? Guid.Empty;
				case EntityType.TaskChecklist:
					var dbTaskChecklist = await this.boardManDbContext.TaskChecklists.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
					return dbTaskChecklist?.Task.List.BoardId ?? Guid.Empty;
				case EntityType.TaskWatcher:
					var dbTaskWatcher = await this.boardManDbContext.TaskWatchers.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
					return dbTaskWatcher?.Task.List.BoardId ?? Guid.Empty;
				case EntityType.TaskAttachment:
					var dbTaskAttachment = await this.boardManDbContext.TaskAttachments.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id && x.DeletedAt == null);
					return dbTaskAttachment?.Task.List.BoardId ?? Guid.Empty;
				default:
					return Guid.Empty;
            }
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

	public class EntityResource
    {
        public Guid Id { get; set; }

        public EntityType Type { get; set; }

		public UserAction? Action { get; set; }
	}
}
