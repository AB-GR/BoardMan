using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface IListManager
	{
		Task<List<List>> GetListsAsync(Guid boardId);	

		Task<List> CreateListAsync(List list);

		Task<List> UpdateListAsync(List list);

		Task DeleteListAsync(Guid listId);
	}

	public class ListManager : IListManager
	{
		private readonly BoardManDbContext dbContext;
		private readonly IMapper mapper;

		public ListManager(BoardManDbContext boardManDbContext, IMapper mapper)
		{
			this.dbContext = boardManDbContext;
			this.mapper = mapper;
		}

		public async Task<List> CreateListAsync(List list)
		{
			var dbList = this.mapper.Map<DbList>(list);
			this.dbContext.Lists.Add(dbList);
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<List>(dbList);
		}

		public async Task<List> UpdateListAsync(List list)
		{
			var dbList = await this.dbContext.Lists.FirstOrDefaultAsync(x => x.Id == list.Id && x.DeletedAt == null).ConfigureAwait(false);
			if(dbList == null)
			{
				throw new EntityNotFoundException($"List with Id {list.Id} not found");
			}

			this.mapper.Map(list, dbList);			
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
			return this.mapper.Map<List>(dbList);
		}

		public async Task<List<List>> GetListsAsync(Guid boardId)
		{
			var dbLists = await this.dbContext.Lists.Where(x => x.BoardId == boardId && x.DeletedAt == null).ToListAsync().ConfigureAwait(false);
			return this.mapper.Map<List<List>>(dbLists);	
		}

		public async Task DeleteListAsync(Guid listId)
		{
			var dbList = await this.dbContext.Lists.FirstOrDefaultAsync(x => x.Id == listId && x.DeletedAt == null).ConfigureAwait(false);
			if (dbList == null)
			{
				throw new EntityNotFoundException($"List with Id {listId} not found");
			}

			dbList.DeletedAt = DateTime.UtcNow;
			await this.dbContext.SaveChangesAsync().ConfigureAwait(false);			
		}
    }
}
