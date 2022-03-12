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
	}
}
