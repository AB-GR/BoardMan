using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class PaymentIntentRequestVM
	{		
		public Guid PlanId { get; set; }

		[Required]
		public string Currency { get; set; }

		public BillingDetails BillingDetails { get; set; }

		public int? UserId { get; set; }
	}
}
