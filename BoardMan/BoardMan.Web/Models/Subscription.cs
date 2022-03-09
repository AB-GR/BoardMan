namespace BoardMan.Web.Models
{
	public class Subscription
	{
		public string Name { get; set; } = null!;

		public DateTime StartedAt { get; set; }

		public DateTime ExpireAt { get; set; }

		public PaymentTransaction PaymentTrasaction { get; set; } = null!;

		public string PlanName { get; set; } = null!;
	}
}
