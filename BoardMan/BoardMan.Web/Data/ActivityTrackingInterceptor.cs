using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BoardMan.Web.Data
{
	public class ActivityTrackingInterceptor : ISaveChangesInterceptor
	{
		private List<DbActivityTracking> activityTrackings = new List<DbActivityTracking>();

		public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
		{
			if (eventData.Context == null)
			{
				return result;
			}

			var boardmanDbContext = (BoardManDbContext)eventData.Context;
			activityTrackings.Clear();
			activityTrackings = CreateActivityTrackingEntry(boardmanDbContext);
			if (activityTrackings.Count > 0)
			{
				foreach (var activityTracking in activityTrackings)
				{
					activityTracking.StartTime = DateTime.UtcNow;
					boardmanDbContext.ActivityTrackings.Add(activityTracking);
				}
				//await boardmanDbContext.SaveChangesAsync();
			}

			return result;
		}

		public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		{
			if (eventData.Context == null)
			{
				return result;
			}

			var boardmanDbContext = (BoardManDbContext)eventData.Context;
			activityTrackings = CreateActivityTrackingEntry(boardmanDbContext);
			if (activityTrackings.Count > 0)
			{
				foreach (var activityTracking in activityTrackings)
				{
					activityTracking.StartTime = DateTime.UtcNow;
					boardmanDbContext.ActivityTrackings.Add(activityTracking);
				}
				//boardmanDbContext.SaveChanges();
			}

			return result;
		}

		public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
		{
			if (eventData.Context == null)
			{
				return result;
			}

			if (activityTrackings.Count > 0)
			{
				var boardmanDbContext = (BoardManDbContext)eventData.Context;
				foreach (var activityTracking in activityTrackings)
				{
					boardmanDbContext.Attach(activityTracking);
					activityTracking.Succeeded = true;
					activityTracking.EndTime = DateTime.UtcNow;
				}

				//boardmanDbContext.SaveChanges();
			}

			return result;
		}

		public async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
		{
			if (eventData.Context == null)
			{
				return result;
			}

			if (activityTrackings.Count > 0)
			{
				var boardmanDbContext = (BoardManDbContext)eventData.Context;
				foreach (var activityTracking in activityTrackings)
				{
					boardmanDbContext.Attach(activityTracking);
					activityTracking.Succeeded = true;
					activityTracking.EndTime = DateTime.UtcNow;
				}

				//await boardmanDbContext.SaveChangesAsync();
			}

			return result;
		}

		public void SaveChangesFailed(DbContextErrorEventData eventData)
		{
			if (eventData.Context == null)
			{
				return;
			}

			if (activityTrackings.Count > 0)
			{
				var boardmanDbContext = (BoardManDbContext)eventData.Context;
				foreach (var activityTracking in activityTrackings)
				{
					boardmanDbContext.Attach(activityTracking);
					activityTracking.Succeeded = false;
					activityTracking.EndTime = DateTime.UtcNow;
				}

				//boardmanDbContext.SaveChanges();
			}

			return;
		}

		public async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
		{
			if (eventData.Context == null)
			{
				return;
			}

			if (activityTrackings.Count > 0)
			{
				var boardmanDbContext = (BoardManDbContext)eventData.Context;
				foreach (var activityTracking in activityTrackings)
				{
					boardmanDbContext.Attach(activityTracking);
					activityTracking.Succeeded = false;
					activityTracking.EndTime = DateTime.UtcNow;
				}

				//await boardmanDbContext.SaveChangesAsync();
			}
			
			return;
		}			

        private static List<DbActivityTracking> CreateActivityTrackingEntry(BoardManDbContext context)
        {
			var activityTrackings = new List<DbActivityTracking>();

			if (context.LoggedInUserId != null)
			{
				context.ChangeTracker.DetectChanges();

				foreach (var entry in context.ChangeTracker.Entries<IActivityTracked>())
				{
					if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
						continue;

					var activityTracking = new DbActivityTracking();
					activityTracking.Action = entry.State == EntityState.Added ? UserAction.Add : entry.State == EntityState.Modified ? UserAction.Update : UserAction.Delete;
					activityTracking.EntityDisplayName = (entry.Entity as IActivityTracked)?.EntityDisplayName;
					var pk = entry.Properties.First(x => x.Metadata.IsPrimaryKey());
					activityTracking.EntityUrn = $"{entry.Metadata.DisplayName()}:{pk.CurrentValue}";

					foreach (var property in entry.Properties)
					{
						var propertyName = property.Metadata.Name;
						if (entry.State == EntityState.Modified && property.IsModified
							&& property.Metadata.PropertyInfo != null && Attribute.IsDefined(property.Metadata.PropertyInfo, typeof(ActivityTrackedAttribute)))
						{
							activityTracking.ChangedProperties.Add(new DbChangedProperty { PropertyName = propertyName, OldValue = property.OriginalValue?.ToString(), NewValue = property.CurrentValue?.ToString() });
						}
					}

					activityTrackings.Add(activityTracking);
				}
			}            

            return activityTrackings;
        }
    }
}
