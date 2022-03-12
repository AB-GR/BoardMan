using AutoMapper;
using BoardMan.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface IWorkspaceManager
	{
		Task CreateOrUpdateWorskpaceAsync(Guid userId, Guid? subscriptionId = null);
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
	}
}
