using AutoMapper;
using BoardMan.Web.Auth;
using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface IWorkspaceManager
	{
		Task CreateOrUpdateWorskpaceAsync(Guid userId, Guid? subscriptionId = null);

		Task<AllWorkspaces> GetAllWorkSpacesAsync(Guid userId);

		Task<List<WorkspaceMember>> ListWorkspaceMembersAsync(Guid workspaceId, Guid currentUserId);

		Task<WorkspaceMember> CreateWorkspaceMemberAsync(WorkspaceMember workspaceMember, Guid currentUserId);

		Task<WorkspaceMember> EditWorkspaceMemberAsync(WorkspaceMember workspaceMember);

		Task DeleteWorkspaceMemberAsync(Guid workspaceMemberId);

		Task<List<UsersOption>> ListProspectiveUsersAsync(Guid currentUserId, Guid workSpaceId);
	}

	public class WorkspaceManager : IWorkspaceManager
	{
		private readonly BoardManDbContext dbContext;
		private readonly ILogger<WorkspaceManager> logger;
		private readonly IMapper mapper;

		public WorkspaceManager(BoardManDbContext dbContext, ILogger<WorkspaceManager> logger, IMapper mapper)
		{
			this.dbContext = dbContext;
			this.logger = logger;
			this.mapper = mapper;
		}

		public async Task CreateOrUpdateWorskpaceAsync(Guid userId, Guid? subscriptionId = null)
		{
			var workspace = await dbContext.Workspaces.Where(w => w.OwnerId == userId).FirstOrDefaultAsync().ConfigureAwait(false);

			if (workspace != null)
			{
				workspace.SubscriptionId = subscriptionId;
			}
			else
			{
				dbContext.Workspaces.Add(new DbWorkspace
				{
					Title = "New Workspace",
					Description = "A workspace to add boards, new members and assign roles.",
					SubscriptionId = subscriptionId,
					OwnerId = userId
				});
			}

			await dbContext.SaveChangesAsync().ConfigureAwait(false);

		}

		public async Task<AllWorkspaces> GetAllWorkSpacesAsync(Guid userId)
		{
			var allWorkspaces = new AllWorkspaces();
			var primary = await this.dbContext.Workspaces.Include(x => x.Boards).FirstAsync(x => x.OwnerId == userId && x.DeletedAt == null).ConfigureAwait(false);
			// TODO: optimize this call
			primary.Boards = primary.Boards.Where(x => x.DeletedAt == null).ToList();

			// TODO: optimize this call
			var otherWorspaces = await this.dbContext.WorkspaceMembers.Where(x => x.MemberId == userId && x.DeletedAt == null).Include(x => x.Workspace.Boards).Select(x => x.Workspace).ToListAsync();
			foreach (var otherWorkspace in otherWorspaces)
			{
				otherWorkspace.Boards = otherWorkspace.Boards.Where(x => x.DeletedAt == null).ToList();
			}

			allWorkspaces.Primary = this.mapper.Map<Workspace>(primary);
			allWorkspaces.Others = this.mapper.Map<List<Workspace>>(otherWorspaces);
			return allWorkspaces;
		}

		public async Task<List<WorkspaceMember>> ListWorkspaceMembersAsync(Guid workspaceId, Guid currentUserId)
		{
			var dbMembers = await this.dbContext.WorkspaceMembers.Where(x => x.WorkspaceId == workspaceId && x.MemberId != currentUserId && x.DeletedAt == null).ToListAsync();
			var workspaceMembers = this.mapper.Map<List<WorkspaceMember>>(dbMembers);
			var entityUrn = $"Workspace:{workspaceId}";
			var invitedMembers = await this.dbContext.EmailInvites.Where(x => x.EntityUrn == entityUrn && x.Accepted == null && x.DeletedAt == null).ToListAsync();
			workspaceMembers.AddRange(this.mapper.Map<List<WorkspaceMember>>(invitedMembers));
			return workspaceMembers;
		}

		public async Task<WorkspaceMember> CreateWorkspaceMemberAsync(WorkspaceMember workspaceMember, Guid currentUserId)
		{
			var existingUserId = workspaceMember.MemberId ?? (await this.dbContext.Users.Where(x => x.UserName == workspaceMember.MemberEmail).FirstOrDefaultAsync())?.Id;

			if (existingUserId.HasValue)
			{
				if (existingUserId == currentUserId)
				{
					throw new InvalidDataCannotProcessException($"Email {workspaceMember.MemberEmail} belongs to the current user");
				}

				if (existingUserId == Users.ApplicationSuperAdminId)
				{
					throw new InvalidDataCannotProcessException($"Email {workspaceMember.MemberEmail} belongs to application super admin");
				}

				var workspace = await this.dbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == workspaceMember.WorkspaceId && x.DeletedAt == null);

				if(workspace == null)
				{
					throw new EntityNotFoundException($"Workspace with id {workspaceMember.WorkspaceId} does not exist");
				}

				if (existingUserId == workspace.OwnerId)
				{
					throw new EntityNotFoundException($"Email {workspaceMember.MemberEmail} belongs to workspace owner");
				}

				// Existing User
				var dbWorkspaceMember = this.mapper.Map<DbWorkspaceMember>(workspaceMember);
				this.dbContext.WorkspaceMembers.Add(dbWorkspaceMember);
				await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
				return this.mapper.Map<WorkspaceMember>(dbWorkspaceMember);
			}
			else
			{
				var dbEmailInvite = this.mapper.Map<DbEmailInvite>(workspaceMember);
				// ToDo: Get this from config
				dbEmailInvite.ExpireAt = DateTime.UtcNow.AddDays(7);
				dbEmailInvite.Token = $"{Guid.NewGuid()}";
				this.dbContext.EmailInvites.Add(dbEmailInvite);
				await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
				return this.mapper.Map<WorkspaceMember>(dbEmailInvite);
			}
		}

		public async Task DeleteWorkspaceMemberAsync(Guid id)
		{
			var dbWorkspaceMember = await this.dbContext.WorkspaceMembers.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
			var dbEmailInvite = await this.dbContext.EmailInvites.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
			
			if (dbWorkspaceMember != null)
				dbWorkspaceMember.DeletedAt = DateTime.UtcNow;
			else if(dbEmailInvite != null)
				dbEmailInvite.DeletedAt = DateTime.UtcNow;
			else
				throw new EntityNotFoundException($"WorkspaceMember or EmailInvite with Id {id} not found");

			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<List<UsersOption>> ListProspectiveUsersAsync(Guid currentUserId, Guid workSpaceId)
		{
			var workspace = await this.dbContext.Workspaces.FirstOrDefaultAsync(x => x.Id == workSpaceId && x.DeletedAt == null);

			if (workspace == null)
			{
				throw new EntityNotFoundException($"Workspace with id {workSpaceId} does not exist");
			}

			var existingUsers = await this.dbContext.Users
				.Where(u => !dbContext.WorkspaceMembers.Where(x => x.WorkspaceId == workSpaceId && x.DeletedAt == null).Select(m => m.MemberId).Contains(u.Id) 
				&& u.Id != Users.ApplicationSuperAdminId && u.Id != currentUserId && u.Id != workspace.OwnerId)
				.Select(x => new UsersOption
				{
					Value = x.Id,
					Label = x.UserName
				})
				.ToListAsync();

			return existingUsers;
		}

		public async Task<WorkspaceMember> EditWorkspaceMemberAsync(WorkspaceMember workspaceMember)
		{			
			var dbWorkspaceMember = await this.dbContext.WorkspaceMembers.FirstOrDefaultAsync(x => x.Id == workspaceMember.Id && x.DeletedAt == null);

			if (dbWorkspaceMember != null)
			{
				if (dbWorkspaceMember.Member.UserName != workspaceMember.MemberEmail)
				{
					throw new InvalidDataCannotProcessException($"Email {workspaceMember.MemberEmail} has changed.");
				}			

				this.mapper.Map(workspaceMember, dbWorkspaceMember);
				await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
				return this.mapper.Map<WorkspaceMember>(dbWorkspaceMember);
			}
			else
			{
				var dbEmailInvite = await this.dbContext.EmailInvites.FirstOrDefaultAsync(x => x.Id == workspaceMember.Id);
				if (dbEmailInvite == null)
				{
					throw new EntityNotFoundException($"EmailInvite with Id {workspaceMember.Id} not found");
				}

				if (dbEmailInvite.EmailAddress != workspaceMember.MemberEmail)
				{
					throw new InvalidDataCannotProcessException($"Email {workspaceMember.MemberEmail} has changed.");
				}

				this.mapper.Map(workspaceMember, dbEmailInvite);
				await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
				return this.mapper.Map<WorkspaceMember>(dbEmailInvite);
			}
		}
	}
}
