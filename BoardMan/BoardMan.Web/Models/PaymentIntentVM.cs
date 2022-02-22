using BoardMan.Web.Data;

namespace BoardMan.Web.Models
{
	public class PaymentIntentVM
	{
		public string PaymentReference { get; set; }

		public PaymentStatus Status { get; set; }

		public string StatusReason { get; set; }

		public Guid PlanId { get; set; }

		public Guid? PlanDiscountId { get; set; }

		public decimal CostBeforeDiscount { get; set; }

		public decimal DiscountApplied { get; set; }

		public decimal FinalCost { get; set; }

		public string Currency { get; set; }		
		
		public Guid TransactedById { get; set; }

		public BillingDetails BillingDetails { get; set; }

		public string Errors { get; set; }
	}
}
