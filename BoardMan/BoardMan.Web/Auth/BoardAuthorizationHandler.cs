﻿using BoardMan.Web.Data;
using BoardMan.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Auth
{
	public class BoardAuthorizationHandler : AuthorizationHandler<BoardAuthorizationrRequirement, EntityResource>
	{
		private readonly BoardManDbContext boardManDbContext;
		private readonly UserManager<AppUser> userManager;

		public BoardAuthorizationHandler(BoardManDbContext boardManDbContext, UserManager<AppUser> userManager) 
		{
			this.boardManDbContext = boardManDbContext;
			this.userManager = userManager;
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BoardAuthorizationrRequirement requirement, EntityResource resource)
		{
			var currentUserId = this.userManager.GetGuidUserId(context.User);
			var boardId = await GetBoardIdAsync(resource);
			var board = await this.boardManDbContext.Boards.Include(x => x.Workspace).FirstOrDefaultAsync(x => x.Id == boardId && x.DeletedAt == null);
			if (board != null && (board.OwnerId == currentUserId || board.Workspace.OwnerId  == currentUserId))
			{
				// Is Board SuperAdmin or Workspace SuperAdmin
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
					var dbBoardMember = await this.boardManDbContext.BoardMembers.FirstOrDefaultAsync(x => x.Id == resource.Id);
					return dbBoardMember?.BoardId ?? Guid.Empty;
				case EntityType.List:
					var dbList = await this.boardManDbContext.Lists.FirstOrDefaultAsync(x => x.Id == resource.Id);
					return dbList?.BoardId ?? Guid.Empty;
				case EntityType.Task:
					var dbTask = await this.boardManDbContext.Tasks.Include(x => x.List).FirstOrDefaultAsync(x => x.Id == resource.Id);
					return dbTask?.List.BoardId ?? Guid.Empty;
				case EntityType.TaskComment:
					var dbTaskComment = await this.boardManDbContext.TaskComments.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id);
					return dbTaskComment?.Task.List.BoardId ?? Guid.Empty;
				case EntityType.Tasklabel:
					var dbTaskLabel = await this.boardManDbContext.TaskLabels.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id);
					return dbTaskLabel?.Task.List.BoardId ?? Guid.Empty;
				case EntityType.TaskChecklist:
					var dbTaskChecklist = await this.boardManDbContext.TaskChecklists.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id);
					return dbTaskChecklist?.Task.List.BoardId ?? Guid.Empty;
				case EntityType.TaskWatcher:
					var dbTaskWatcher = await this.boardManDbContext.TaskWatchers.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id);
					return dbTaskWatcher?.Task.List.BoardId ?? Guid.Empty;
				case EntityType.TaskAttachment:
					var dbTaskAttachment = await this.boardManDbContext.TaskAttachments.Include(x => x.Task.List).FirstOrDefaultAsync(x => x.Id == resource.Id);
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
