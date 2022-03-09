namespace BoardMan.Web.Models
{
	public class PaymentIntentResponse
	{
		public string PaymentIntentId { get; set; }

		public string ClientSecret { get; set; }

		public string BusinessName { get; set; }

		public string ProductName { get; set; }

		public string ProductCode { get; set; }

		public long Amount { get; set; }

		public string Currency { get; set; }
	}
}
