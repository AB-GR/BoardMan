using AutoMapper;
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

		Task<List<ComboOption>> ListBoardMembersForDisplayAsync(Guid boardId, Guid currentUserId);

		Task<List<BoardMember>> ListBoardMembersAsync(Guid boardId, Guid currentUserId);

		Task<IEnumerable<ComboOption>> ListOtherListsForDisplayAsync(Guid boardId, Guid currentListId);
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

		public async Task DeleteBoardAsync(Guid boardId)
		{
			var dbBoard = await this.dbContext.Boards.FirstOrDefaultAsync(x => x.Id == boardId).ConfigureAwait(false);
			if(dbBoard == null)
			{
				throw new EntityNotFoundException($"Board with Id {boardId} not found");
			}

			dbBoard.DeletedAt = DateTime.UtcNow;
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

		public async Task<List<BoardMember>> ListBoardMembersAsync(Guid boardId, Guid currentUserId)
		{
			var dbMembers = await this.dbContext.BoardMembers.Where(x => x.BoardId == boardId && x.MemberId != currentUserId).Select(x => x.Member).ToListAsync();
			var entityUrn = $"Board:{boardId}";
			var invitedMembers = await this.dbContext.EmailInvites.Where(x => x.EntityUrn == entityUrn && x.Accepted == null && x.DeletedAt == null).ToListAsync();
			return null;
		}

		// ToDo should only load those members who atleast have readwrite access
		public async Task<List<ComboOption>> ListBoardMembersForDisplayAsync(Guid boardId, Guid currentUserId)
		{
			var members = await this.dbContext.BoardMembers.Where(x => x.BoardId == boardId && x.MemberId != currentUserId).Select(x => new ComboOption { Value = x.Member.Id, DisplayText = x.Member.UserName }).ToListAsync();			
			members.Insert(0, new ComboOption { Value = Guid.Empty, DisplayText = "Select a user" });
			return members;
		}

		public async Task<IEnumerable<ComboOption>> ListOtherListsForDisplayAsync(Guid boardId, Guid currentListId)
		{
			var otherLists = await this.dbContext.Lists.Where(x => x.BoardId == boardId && x.Id != currentListId && x.DeletedAt == null).Select(x => new ComboOption { Value = x.Id, DisplayText = x.Title }).ToListAsync();
			otherLists.Insert(0, new ComboOption { Value = Guid.Empty, DisplayText = "Select another list" });
			return otherLists;
		}
	}
}
