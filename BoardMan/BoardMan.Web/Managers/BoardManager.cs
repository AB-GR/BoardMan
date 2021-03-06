using AutoMapper;
using BoardMan.Web.Auth;
using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface IBoardManager
	{
		Task<Board> GetBoardAsync(Guid boardId);

		Task CreateBoardAsync(Board board, Guid ownerId);		

		Task DeleteBoardAsync(Guid boardId);

		Task<List<ComboOption>> ListWatchersForDisplayAsync(Guid boardId);

		Task<List<ComboOption>> ListAssigneesForDisplayAsync(Guid boardId);

		Task<IEnumerable<ComboOption>> ListOtherListsForDisplayAsync(Guid boardId, Guid currentListId);

		Task<List<BoardMember>> ListBoardMembersAsync(Guid boardId, Guid currentUserId);

		Task<BoardMember> CreateBoardMemberAsync(BoardMember boardMember, Guid currentUserId);

		Task<BoardMember> EditBoardMemberAsync(BoardMember boardMember, Guid currentUserId);

		Task DeleteBoardMemberAsync(Guid boardMemberId);

		Task<List<UsersOption>> ListProspectiveUsersAsync(Guid currentUserId, Guid boardId);
	}

	public class BoardManager : IBoardManager
	{
		private readonly BoardManDbContext dbContext;
		private readonly IMapper mapper;

		public BoardManager(BoardManDbContext dbContext, IMapper mapper)
		{
			this.dbContext = dbContext;
			this.mapper = mapper;
		}

		public async Task CreateBoardAsync(Board board, Guid ownerId)
		{
			var dbBoard = this.mapper.Map<DbBoard>(board);			
			dbBoard.OwnerId = ownerId;
			dbContext.Boards.Add(dbBoard);
			await dbContext.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<Board> GetBoardAsync(Guid boardId)
		{
			var dbBoard = await this.dbContext.Boards.FirstOrDefaultAsync(x => x.Id == boardId).ConfigureAwait(false);
			if (dbBoard == null)
			{
				throw new EntityNotFoundException($"Board with Id {boardId} not found");
			}

			return this.mapper.Map<Board>(dbBoard);
		}

		public async Task DeleteBoardAsync(Guid boardId)
		{
			var dbBoard = await this.dbContext.Boards.FirstOrDefaultAsync(x => x.Id == boardId).ConfigureAwait(false);
			if (dbBoard == null)
			{
				throw new EntityNotFoundException($"Board with Id {boardId} not found");
			}

			dbBoard.DeletedAt = DateTime.UtcNow;
			await dbContext.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<List<BoardMember>> ListBoardMembersAsync(Guid boardId, Guid currentUserId)
		{
			var dbMembers = await this.dbContext.BoardMembers.Where(x => x.BoardId == boardId && x.MemberId != currentUserId && x.DeletedAt == null).ToListAsync();
			var boardMembers = this.mapper.Map<List<BoardMember>>(dbMembers);
			var entityUrn = $"Board:{boardId}";
			var invitedMembers = await this.dbContext.EmailInvites.Where(x => x.EntityUrn == entityUrn && x.Accepted == null && x.DeletedAt == null).ToListAsync();
			boardMembers.AddRange(this.mapper.Map<List<BoardMember>>(invitedMembers));
			return boardMembers;
		}

		public async Task<BoardMember> CreateBoardMemberAsync(BoardMember boardMember, Guid currentUserId)
		{
			var existingUserId = boardMember.MemberId ?? (await this.dbContext.Users.Where(x => x.UserName == boardMember.MemberEmail).FirstOrDefaultAsync())?.Id;
			
			if(existingUserId.HasValue)
			{				
				if(existingUserId == currentUserId)
				{
					throw new InvalidDataCannotProcessException($"Email {boardMember.MemberEmail} belongs to the current user");
				}

				if (existingUserId == Users.ApplicationSuperAdminId)
				{
					throw new InvalidDataCannotProcessException($"Email {boardMember.MemberEmail} belongs to application super admin");
				}

				var board = await this.dbContext.Boards.FirstOrDefaultAsync(x => x.Id == boardMember.BoardId && x.DeletedAt == null);

				if (board == null)
				{
					throw new EntityNotFoundException($"Board with id {boardMember.BoardId} does not exist");
				}

				if (existingUserId == board.OwnerId)
				{
					throw new EntityNotFoundException($"Email {boardMember.MemberEmail} belongs to board owner");
				}

				// Existing User
				var dbBoardMember = this.mapper.Map<DbBoardMember>(boardMember);
				this.dbContext.BoardMembers.Add(dbBoardMember);
				await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
				return this.mapper.Map<BoardMember>(dbBoardMember);
			}
			else
			{
				var dbEmailInvite = this.mapper.Map<DbEmailInvite>(boardMember);
				// ToDo: Get this from config
				dbEmailInvite.ExpireAt = DateTime.UtcNow.AddDays(7);
				dbEmailInvite.Token = $"{Guid.NewGuid()}";
				this.dbContext.EmailInvites.Add(dbEmailInvite);
				await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
				return this.mapper.Map<BoardMember>(dbEmailInvite);
			}
		}

		public async Task DeleteBoardMemberAsync(Guid boardMemberId)
		{
			var dbBoardMember = await this.dbContext.BoardMembers.FirstOrDefaultAsync(x => x.Id == boardMemberId && x.DeletedAt == null);
			var dbEmailInvite = await this.dbContext.EmailInvites.FirstOrDefaultAsync(x => x.Id == boardMemberId && x.DeletedAt == null);
			
			if(dbBoardMember != null)
				dbBoardMember.DeletedAt = DateTime.UtcNow;
			else if(dbEmailInvite != null)
				dbEmailInvite.DeletedAt = DateTime.UtcNow;
			else
				throw new EntityNotFoundException($"BoardMember or EmailInvite with Id {boardMemberId} not found");

			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
		}
		
		public async Task<List<ComboOption>> ListWatchersForDisplayAsync(Guid boardId)
		{
			var members = await this.dbContext.BoardMembers.Where(x => x.BoardId == boardId && x.DeletedAt == null).Select(x => new ComboOption { Value = x.Member.Id, DisplayText = x.Member.UserName }).ToListAsync();			
			members.Insert(0, new ComboOption { Value = Guid.Empty, DisplayText = "Select a user" });
			return members;
		}

		public async Task<List<ComboOption>> ListAssigneesForDisplayAsync(Guid boardId)
		{
			var applicableRoles = new List<string> { Roles.BoardAdmin, Roles.BoardContributor, Roles.BoardSuperAdmin };
			var board = await this.dbContext.Boards.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == boardId && x.DeletedAt == null).ConfigureAwait(false);
			if(board == null)
			{
				throw new EntityNotFoundException($"Board with Id {boardId} not found");
			}

			var members = await this.dbContext.BoardMembers.Where(x => x.BoardId == boardId && applicableRoles.Contains(x.Role.Name) && x.DeletedAt == null).Select(x => new ComboOption { Value = x.Member.Id, DisplayText = x.Member.UserName }).ToListAsync();
			members.Insert(0, new ComboOption { Value = board.Owner.Id, DisplayText = board.Owner.UserName });
			members.Insert(0, new ComboOption { Value = Guid.Empty, DisplayText = "Select a user" });
			return members;
		}

		public async Task<IEnumerable<ComboOption>> ListOtherListsForDisplayAsync(Guid boardId, Guid currentListId)
		{
			var otherLists = await this.dbContext.Lists.Where(x => x.BoardId == boardId && x.Id != currentListId && x.DeletedAt == null).Select(x => new ComboOption { Value = x.Id, DisplayText = x.Title }).ToListAsync();
			otherLists.Insert(0, new ComboOption { Value = Guid.Empty, DisplayText = "Select another list" });
			return otherLists;
		}

		public async Task<List<UsersOption>> ListProspectiveUsersAsync(Guid currentUserId, Guid boardId)
		{
			var board = await this.dbContext.Boards.FirstOrDefaultAsync(x => x.Id == boardId && x.DeletedAt == null);
			if (board == null)
			{
				throw new EntityNotFoundException($"Board with id {boardId} does not exist");
			}

			var existingUsers = await dbContext.Users
				.Where(x => !dbContext.BoardMembers.Where(x => x.BoardId == boardId && x.DeletedAt == null).Select(m => m.MemberId).Contains(x.Id) && x.Id != currentUserId && x.Id != Users.ApplicationSuperAdminId
				&& x.Id != board.OwnerId)
				.Select(x => new UsersOption
				{
					Value = x.Id,
					Label = x.UserName
				}).ToListAsync();

			return existingUsers;
		}

		public async Task<BoardMember> EditBoardMemberAsync(BoardMember boardMember, Guid currentUserId)
		{
			var dbBoardMember = await this.dbContext.BoardMembers.FirstOrDefaultAsync(x => x.Id == boardMember.Id && x.DeletedAt == null);

			if (dbBoardMember != null)
			{
				if (dbBoardMember.Member.UserName != boardMember.MemberEmail)
				{
					throw new InvalidDataCannotProcessException($"Email {boardMember.MemberEmail} has changed.");
				}

				this.mapper.Map(boardMember, dbBoardMember);
				await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
				return this.mapper.Map<BoardMember>(dbBoardMember);
			}
			else
			{
				var dbEmailInvite = await this.dbContext.EmailInvites.FirstOrDefaultAsync(x => x.Id == boardMember.Id);
				if (dbEmailInvite == null)
				{
					throw new EntityNotFoundException($"EmailInvite with Id {boardMember.Id} not found");
				}

				if (dbEmailInvite.EmailAddress != boardMember.MemberEmail)
				{
					throw new InvalidDataCannotProcessException($"Email {boardMember.MemberEmail} has changed.");
				}

				this.mapper.Map(boardMember, dbEmailInvite);
				await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
				return this.mapper.Map<BoardMember>(dbEmailInvite);
			}
		}
	}
}
