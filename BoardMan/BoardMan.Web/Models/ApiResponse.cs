namespace BoardMan.Web.Models
{
	public class ApiResponse
	{
		public string Result => this.Succeeded ? "OK" : "ERROR";

		public bool Succeeded { get; protected set; }

		public string Message { get; protected set; }

		public static ApiResponse Error(string errorMessage, params object[] args) => new ErrorApiResponse(string.Format(errorMessage, args));

		public static ApiResponse Success(string message = null) => new SuccessApiResponse(message);

		public static ApiResponse Single(object record) => new SuccessSingleApiResponse(record);

		public static ApiResponse List(IEnumerable<object> records) => List(records, records.Count());

		public static ApiResponse ListOptions(IEnumerable<object> options) => new SuccessOptionsApiResponse(options);

		public static ApiResponse List(IEnumerable<object> records, int totalRecordsCount) => new SuccessListApiResponse(records, totalRecordsCount);
	}

	public class ErrorApiResponse : ApiResponse
	{
		public ErrorApiResponse(string message)
		{
			this.Message = message;
			this.Succeeded = false;
		}
	}

	public class SuccessApiResponse : ApiResponse
	{
		public SuccessApiResponse()
			: this(null)
		{
		}

		public SuccessApiResponse(string message)
		{
			this.Message = message;
			this.Succeeded = true;
		}
	}

	public class SuccessSingleApiResponse : SuccessApiResponse
	{
		public object Record { get; }

		public SuccessSingleApiResponse(object record)
		{
			this.Record = record;
		}
	}

	public class SuccessListApiResponse : SuccessApiResponse
	{
		public IEnumerable<object> Records { get; }
		public int TotalRecordCount { get; }

		public SuccessListApiResponse(IEnumerable<object> records, int totalRecordCount)
		{
			this.Records = records;
			this.TotalRecordCount = totalRecordCount;
		}
	}

	public class SuccessOptionsApiResponse : SuccessApiResponse
	{
		public IEnumerable<object> Options { get; }

		public SuccessOptionsApiResponse(IEnumerable<object> options)
		{
			this.Options = options;
		}
	}
}
