namespace BoardMan.Web.Models
{
    public class SubscriptionNotificationVM
    {
        public SubscriptionStatus SubscriptionStatus { get; set; }

        public Guid? PriorPlanId { get; set; }
    }

    public enum SubscriptionStatus
    {
        NoSubscription, SubscriptionAboutToExpire, SubscriptionExpiredPriosubscriptionValid, SubscriptionExpiredPriosubscriptionInValid
    }
}
