using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace BoardMan.Web.Infrastructure.Extensions
{
	public static class ViewExtensions
	{
		private static readonly Regex _valueAttributeCapture = new Regex(@"value=[""'](.*?)['""]", RegexOptions.IgnoreCase);
		
		public static string AntiForgeryTokenValue(this IHtmlHelper html)
		{
			var match = _valueAttributeCapture.Match(html.AntiForgeryToken().ToHtmlString());
			return match.Success ? match.Groups[1].Value : string.Empty;
		}

		public static string ToHtmlString(this IHtmlContent htmlContent)
		{
			if (htmlContent is HtmlString htmlString)
			{
				return htmlString.Value ?? string.Empty;
			}

			using (var writer = new StringWriter())
			{
				htmlContent.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
				return writer.ToString();
			}
		}
	}
}
