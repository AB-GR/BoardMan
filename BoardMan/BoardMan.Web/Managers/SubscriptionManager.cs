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

        Task<List<PaymentTransaction>> GetPaymentTransactions(Guid subscriptionId);
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

		public async Task<List<PaymentTransaction>> GetPaymentTransactions(Guid subscriptionId)
		{
            var dbPaymentTransactions = await _dbContext.Subscriptions
                .Where(x => x.Id == subscriptionId && x.DeletedAt == null)
                .Include(x => x.PaymentTrasaction.Plan)
                .Include(x => x.PaymentTrasaction.TransactedBy)
                .OrderByDescending(x => x.ExpireAt)
                .Select(x => x.PaymentTrasaction).ToListAsync();

            return this.mapper.Map<List<PaymentTransaction>>(dbPaymentTransactions);
        }

		public async Task<SubscriptionNotification> GetSubscriptionNotificationAsync(Guid userId)
        {
            var latestSubscription = await _dbContext.Subscriptions
                .Where(x => x.OwnerId == userId && x.DeletedAt == null)
                .Include(x => x.PaymentTrasaction.Plan)
                .Include(x => x.PaymentTrasaction.TransactedBy)
                .OrderByDescending(x => x.ExpireAt).FirstOrDefaultAsync();

            var priorPlanId = latestSubscription != null && !latestSubscription.PaymentTrasaction.Plan.Expired ?
                                latestSubscription.PaymentTrasaction.PlanId : (Guid?)null;

            var result = new SubscriptionNotification
            {
                SubscriptionStatus = latestSubscription == null ?
                SubscriptionStatus.NotAvailable :
                    !latestSubscription.Expired ?
                        latestSubscription.ExpireAt.Subtract(DateTime.UtcNow).Days <= this.configuration.GetValue("SubscriptionAboutToExpireDays", 7) ?
                            priorPlanId == null ? SubscriptionStatus.AboutToExpirePriorPlanInvalid : SubscriptionStatus.AboutToExpire
                                : SubscriptionStatus.Valid
                    : priorPlanId == null ? SubscriptionStatus.ExpiredPriorPlanInvalid : SubscriptionStatus.Expired,
                PriorPlanId = priorPlanId,
                HasOtherWorkspaces = await _dbContext.WorkspaceMembers.AnyAsync(x => x.MemberId == userId && x.DeletedAt == null).ConfigureAwait(false)
            };

            return result;
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
