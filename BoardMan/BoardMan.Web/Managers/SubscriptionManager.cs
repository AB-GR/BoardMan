using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
    public interface ISubscriptionManager
    {
        Task<SubscriptionNotification> GetSubscriptionNotificationAsync(Guid userId);

        Task<List<Subscription>> GetSubscriptionsAsync(Guid userId);
    }

    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly BoardManDbContext _dbContext;
		private readonly IConfiguration configuration;
		private readonly IMapper mapper;

		public SubscriptionManager(BoardManDbContext dbContext, IConfiguration configuration, IMapper mapper)
        {
            _dbContext = dbContext;
			this.configuration = configuration;
			this.mapper = mapper;
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

		public async Task<List<Subscription>> GetSubscriptionsAsync(Guid userId)
		{
            var dbSubscriptions = await _dbContext.Subscriptions
                .Where(x => x.OwnerId == userId && x.DeletedAt == null)
                .Include(x => x.PaymentTrasaction.Plan)
                .OrderByDescending(x => x.ExpireAt).ToListAsync();

            return this.mapper.Map<List<Subscription>>(dbSubscriptions);
        }
	}
}
