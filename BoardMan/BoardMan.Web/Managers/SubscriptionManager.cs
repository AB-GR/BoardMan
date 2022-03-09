using BoardMan.Web.Data;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
    public interface ISubscriptionManager
    {
        Task<SubscriptionNotification> GetSubscriptionNotificationAsync(Guid userId);
    }

    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly BoardManDbContext _dbContext;
		private readonly IConfiguration configuration;

		public SubscriptionManager(BoardManDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
			this.configuration = configuration;
		}

        public async Task<SubscriptionNotification> GetSubscriptionNotificationAsync(Guid userId)
        {
            var latestSubscription = await _dbContext.Subscriptions
                .Where(x => x.OwnerId == userId && x.DeletedAt == null)
                .Include(x => x.PaymentTrasaction.Plan)
                .OrderByDescending(x => x.ExpireAt).FirstOrDefaultAsync();			

			return new SubscriptionNotification
            {
                SubscriptionStatus = latestSubscription == null ? 
                SubscriptionStatus.NoSubscriptionAvailable :
                    !latestSubscription.Expired ? 
                        latestSubscription.ExpireAt.Subtract(DateTime.UtcNow).Days <= this.configuration.GetValue("SubscriptionAboutToExpireDays", 7) ? 
                            SubscriptionStatus.SubscriptionAboutToExpire 
                                : SubscriptionStatus.SubscriptionValid
                    : SubscriptionStatus.SubscriptionExpired,

                PriorPlanId = latestSubscription != null && !latestSubscription.PaymentTrasaction.Plan.Expired ? 
                                latestSubscription.PaymentTrasaction.PlanId : null
            }; ;
        }
    }
}
