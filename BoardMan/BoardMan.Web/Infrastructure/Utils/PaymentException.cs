namespace BoardMan.Web.Infrastructure.Utils
{
	public class PaymentException : Exception
	{
		public PaymentException(string? message) : base(message)
		{
		}
	}
}
