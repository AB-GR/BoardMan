using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class PaymentSuccessRequest
	{		
		public string PaymentIntentId { get; set; }

		public Guid? UserId { get; set; }

		public BillingDetails BillingDetails { get; set; }
	}
}
