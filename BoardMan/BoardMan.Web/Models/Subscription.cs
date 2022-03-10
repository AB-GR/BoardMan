using BoardMan.Web.Infrastructure.Converters;
using Newtonsoft.Json;

namespace BoardMan.Web.Models
{
	public class Subscription
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = null!;

		[JsonConverter(typeof(FormattedDateTimeZonePropertyConverter), DateTimeFormats.DateFormat)]
		public DateTime StartedAt { get; set; }

		[JsonConverter(typeof(FormattedDateTimeZonePropertyConverter), DateTimeFormats.DateFormat)]
		public DateTime ExpireAt { get; set; }

		public PaymentTransaction PaymentTrasaction { get; set; } = null!;

		public string PlanName { get; set; } = null!;
	}
}
