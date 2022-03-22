namespace BoardMan.Web.Infrastructure.Utils
{
	public class InsufficientDataToProcessException : Exception
	{
		public InsufficientDataToProcessException(string? message) : base(message)
		{
		}
	}

	public class EntityNotFoundException : Exception
	{
		public EntityNotFoundException(string? message) : base(message)
		{
		}
	}

	public class InvalidDataCannotProcessException : Exception
	{
		public InvalidDataCannotProcessException(string? message) : base(message)
		{
		}
	}
}
