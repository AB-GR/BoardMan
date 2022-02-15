namespace BoardMan.Web.Infrastructure.Utils
{
	public class InsufficientDataToProcessException : Exception
	{
		public InsufficientDataToProcessException(string? message) : base(message)
		{
		}
	}
}
