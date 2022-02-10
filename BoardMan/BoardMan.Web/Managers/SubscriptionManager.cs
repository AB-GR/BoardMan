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
            var latestSubscription = await _dbContext.Subscriptions
                .Where(x => x.UserId == userId && x.DeletedAt.GetValueOrDefault() == DateTime.MinValue)
                .Include(x => x.PaymentTrasaction.Plan)
                .OrderByDescending(x => x.ExpireAt).FirstOrDefaultAsync();			

			return new SubscriptionNotificationVM
            {
                SubscriptionStatus = latestSubscription == null ? 
                SubscriptionStatus.NoSubscriptionAvailable :
                    !latestSubscription.Expired ? 
                        latestSubscription.ExpireAt.Subtract(DateTime.UtcNow).Days <= 7 ? 
                            SubscriptionStatus.SubscriptionAboutToExpire 
                                : SubscriptionStatus.SubscriptionValid
                    : SubscriptionStatus.SubscriptionExpired,

                PriorPlanId = latestSubscription != null && !latestSubscription.PaymentTrasaction.Plan.Expired ? 
                                latestSubscription.PaymentTrasaction.PlanId : null
            }; ;
        }
    }
}
