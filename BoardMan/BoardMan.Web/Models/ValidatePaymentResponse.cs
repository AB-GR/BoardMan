namespace BoardMan.Web.Models
{
	public class ValidatePaymentResponse
	{
		public bool CanProceed { get; set; }

		public bool ExistingUser { get; set; }

		public string Message { get; set; }
	}
}
