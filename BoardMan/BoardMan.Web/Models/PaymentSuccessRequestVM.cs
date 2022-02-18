using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class PaymentSuccessRequestVM
	{
		[Required]
		public string PaymentIntentId { get; set; }
	}
}
