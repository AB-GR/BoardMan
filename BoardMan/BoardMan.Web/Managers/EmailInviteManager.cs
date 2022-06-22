using BoardMan.Web.Auth;
using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface IEmailInviteManager
	{
		Task<EmailInviteModel> ValidateToken(string token);

		Task CreateMember(string token, Guid memberId); 
	}

	public class EmailInviteManager : IEmailInviteManager
	{
		private readonly BoardManDbContext dbContext;

		public EmailInviteManager(BoardManDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task CreateMember(string token, Guid memberId)
		{
			var dbEmailInvite = await this.dbContext.EmailInvites.SingleOrDefaultAsync(x => x.Token == token);
			if (dbEmailInvite == null)
			{
				throw new EntityNotFoundException($"EmailInvite for {token} does not exist");
			}
			
			var entityUrn = dbEmailInvite.EntityUrn.ToEntityUrn();
			if (entityUrn.EntityName == "Workspace")
			{
				if (!await this.dbContext.Workspaces.AnyAsync(x => x.Id == entityUrn.EntityId && x.DeletedAt == null))
				{
					throw new EntityNotFoundException($"Workspace with Id {entityUrn.EntityId} does not exist");
				}

				dbEmailInvite.Accepted = true;
				var workspaceMember = new DbWorkspaceMember { AddedById = dbEmailInvite.AddedById, RoleId = dbEmailInvite.RoleId, WorkspaceId = entityUrn.EntityId, MemberId = memberId };
				this.dbContext.WorkspaceMembers.Add(workspaceMember);
				await this.dbContext.SaveChangesAsync();
			}
			else if (entityUrn.EntityName == "Board")
			{
				var dbBoard = await this.dbContext.Boards.FirstOrDefaultAsync(x => x.Id == entityUrn.EntityId && x.DeletedAt == null).ConfigureAwait(false);
				if (dbBoard == null)
				{
					throw new EntityNotFoundException($"Board with Id {entityUrn.EntityId} does not exist");
				}

				dbEmailInvite.Accepted = true;
				var dbBoardMember = new DbBoardMember { AddedById = dbEmailInvite.AddedById, RoleId = dbEmailInvite.RoleId, BoardId = entityUrn.EntityId, MemberId = memberId };
				this.dbContext.BoardMembers.Add(dbBoardMember);

				// If there doest exist a workspace member with this user Id add one with a reader role
				var wsReaderRoleId = this.dbContext.Roles.FirstOrDefault(x => x.Name == Roles.WorkspaceReader)?.Id;
				if(wsReaderRoleId == null)
				{
					throw new EntityNotFoundException($"WorkspaceReader role with Name {Roles.WorkspaceReader} does not exist");
				}

				var dbWorkspaceMember = new DbWorkspaceMember { AddedById = dbEmailInvite.AddedById, RoleId = wsReaderRoleId.Value, WorkspaceId = dbBoard.WorkspaceId, MemberId = memberId };
				this.dbContext.WorkspaceMembers.Add(dbWorkspaceMember);

				await this.dbContext.SaveChangesAsync();
			}
		}

		public async Task<EmailInviteModel> ValidateToken(string token)
		{
			var result = new EmailInviteModel { Token = token };
			var dbEmailInvite = await this.dbContext.EmailInvites.SingleOrDefaultAsync(x => x.Token == token && x.DeletedAt == null);
			if(dbEmailInvite == null)
			{
				result.ValidationMessage = $"EmailInvite for {token} does not exist";
			}
			else
			{
				if (dbEmailInvite.ExpireAt < DateTime.UtcNow)
				{
					result.ValidationMessage = $"EmailInvite expired on {dbEmailInvite.ExpireAt}";
				}

				if (dbEmailInvite.Accepted == true)
				{
					result.ValidationMessage = $"EmailInvite has been accepted on {dbEmailInvite.ModifiedAt}";
				}
				
				var entityUrn = dbEmailInvite.EntityUrn.ToEntityUrn();
				if (entityUrn.EntityName == "Board")
				{
					if (!await this.dbContext.Boards.AnyAsync(x => x.Id == entityUrn.EntityId && x.DeletedAt == null))
					{
						result.ValidationMessage = $"Board with Id {entityUrn.EntityId} does not exist";
					}
				}
				else if (entityUrn.EntityName == "Workspace")
				{
					if (!await this.dbContext.Workspaces.AnyAsync(x => x.Id == entityUrn.EntityId && x.DeletedAt == null))
					{
						result.ValidationMessage = $"Workspace with Id {entityUrn.EntityId} does not exist";
					}
				}

				if (await this.dbContext.Users.AnyAsync(x => x.UserName == dbEmailInvite.EmailAddress))
				{
					result.ValidationMessage = $"User with email {dbEmailInvite.EmailAddress} already exists";
				}

				result.Email = dbEmailInvite.EmailAddress;
			}

			if (string.IsNullOrWhiteSpace(result.ValidationMessage))
			{
				result.IsTokenValid = true;
			}
			
			return result;
		}
	}
}
