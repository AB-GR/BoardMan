using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Converters;
using Newtonsoft.Json;

namespace BoardMan.Web.Models
{
	public class PaymentTransaction
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
		
		public Guid? TransactedById { get; set; }

		public BillingDetails BillingDetails { get; set; }

		public string RawData { get; set; }

		public string Errors { get; set; }

		public string? TransactedBy { get; set; }

		[JsonConverter(typeof(FormattedDateTimeZonePropertyConverter), DateTimeFormats.DateFormat)]
		public DateTime? CreatedAt { get; set; }
	}
}
