using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class PaymentIntentRequest
	{		
		public Guid PlanId { get; set; }

		public string Currency { get; set; }

		public BillingDetails? BillingDetails { get; set; }

		public Guid? UserId { get; set; }

		public decimal Cost { get; set; }

		public string? ProductName { get; set; }

		public string? BusinessName { get; set; }

		public string? PlanName { get; set; }

		public string? PlanDescription { get; set; }
	}
}
