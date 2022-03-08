namespace BoardMan.Web.Models
{
    public class SubscriptionNotification
    {
        public SubscriptionStatus SubscriptionStatus { get; set; }

        public Guid? PriorPlanId { get; set; }
    }

    public enum SubscriptionStatus
    {
        NoSubscriptionAvailable, 
        SubscriptionValid,
        SubscriptionAboutToExpire, 
        SubscriptionExpired
    }
}
