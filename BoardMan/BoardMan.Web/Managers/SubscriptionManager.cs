using BoardMan.Web.Data;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
    public interface ISubscriptionManaer
    {
        Task<SubscriptionNotificationVM> GetSubscriptionNotificationAsync(Guid userId);
    }

    public class SubscriptionManager : ISubscriptionManaer
    {
        private readonly BoardManDbContext _dbContext;
        public SubscriptionManager(BoardManDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SubscriptionNotificationVM> GetSubscriptionNotificationAsync(Guid userId)
        {
            var latestSubscription = await _dbContext.Subscriptions.Where(x => x.UserId == userId && 
            x.DeletedAt.GetValueOrDefault() == DateTime.MinValue).OrderByDescending(x => x.ExpireAt).FirstOrDefaultAsync();

            return new SubscriptionNotificationVM ();
        }
    }
}
