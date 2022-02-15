using Microsoft.AspNetCore.Mvc;

namespace BoardMan.Web.Infrastructure.Utils
{
	public class ControllerUtils
	{
		public static string GetControllerName<T>() where T : Controller
		{
			return typeof(T).Name.Replace(nameof(Controller), string.Empty);
		}
	}
}
