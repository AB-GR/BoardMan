using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class RegisterInfoRequest
	{
		public Guid? UserId { get; set; }

		public BillingDetails BillingDetails { get; set; }
	}

	public class PaymentSuccessRequest : RegisterInfoRequest
	{		
		public string PaymentIntentId { get; set; }
		
	}
}
