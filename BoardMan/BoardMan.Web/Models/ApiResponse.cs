﻿namespace BoardMan.Web.Models
{
	public class ApiResponse
	{
		public bool Succeeded { get; protected set; }

		public string Message { get; protected set; }

		public static ApiResponse Error(string errorMessage, params object[] args) => new ErrorApiResponse(string.Format(errorMessage, args));

		public static ApiResponse Success(string message = null) => new SuccessApiResponse(message);

		public static ApiResponse Single(object record) => new SuccessSingleApiResponse(record);

		public static ApiResponse List(IEnumerable<object> records) => List(records, records.Count());

		public static ApiResponse List(IEnumerable<object> records, int totalRecordsCount) => new SuccessListApiResponse(records, totalRecordsCount);
	}

	public class ErrorApiResponse : ApiResponse
	{
		public ErrorApiResponse(string message)
		{
			this.Message = message;
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
}