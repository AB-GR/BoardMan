using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BoardMan.Web.Infrastructure.Utils
{
	public class NotFoundViewResult : ViewResult
	{
		public NotFoundViewResult(string viewName)
		{
			ViewName = viewName;
			StatusCode = (int)HttpStatusCode.NotFound;
		}
	}
}
