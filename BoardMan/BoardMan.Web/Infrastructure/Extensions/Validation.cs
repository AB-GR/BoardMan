using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BoardMan.Web.Infrastructure.Utils.Extensions
{
	public static class Validation
	{
		public static string Errors(this ModelStateDictionary modelState)
		{
			var errors = (from modelStateValue in modelState.Values
						  from modelError in modelStateValue.Errors
						  select modelError.ErrorMessage).ToList();

			return string.Join(",", errors);
		}
	}
}
