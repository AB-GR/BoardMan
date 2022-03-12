using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface IWorkspaceManager
	{
		Task CreateOrUpdateWorskpaceAsync(Guid userId, Guid? subscriptionId = null);

		Task<AllWorkspaces> GetAllWorkSpacesAsync(Guid userId);
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
	}
}
