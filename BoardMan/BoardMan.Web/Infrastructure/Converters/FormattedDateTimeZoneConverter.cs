using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace BoardMan.Web.Infrastructure.Converters
{
	public class FormattedDateTimeZonePropertyConverter : IsoDateTimeConverter
	{
		private readonly string format;

		public FormattedDateTimeZonePropertyConverter(string format)
		{
			this.format = format;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (serializer.Converters?.Any() ?? false)
			{
				foreach (var c in serializer.Converters)
				{
					if (c is FormattedDateTimeZoneConverter ftc)
					{
						ftc.WriteJson(writer, value, serializer, this.format);
						return;
					}
				}
			}

			throw new InvalidOperationException("Timezone converter not found in serializer settings.");
		}
	}

	public class FormattedDateTimeZoneConverter : IsoDateTimeConverter
    {
        CultureInfo current { get; set; }

        public FormattedDateTimeZoneConverter(CultureInfo current)
        {
            this.current = current;
            base.DateTimeFormat = this.current.DateTimeFormat.FullDateTimePattern;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime? temp = (DateTime?)value;
            if (temp != null && temp.HasValue)
            {
                //value = this.current.ToUserDateTime(temp.Value);
            }

            base.WriteJson(writer, value, serializer);
        }

		public void WriteJson(JsonWriter writer, object value, JsonSerializer serializer, string format)
		{
			try
			{
				base.DateTimeFormat = format;
				WriteJson(writer, value, serializer);
			}
			finally
			{
				base.DateTimeFormat = DateTimeFormats.ShortDateTimeFormat;
			}
		}
	}

	public static class DateTimeFormats
	{
		public const string ShortDateTimeFormat = "MMM d, yyy hh:mm tt";
		public const string DateTimeFormat = "MMM dd, yyy hh:mm tt";
		public const string DateFormat = "MMM dd, yyyy";
		public const string RecentTimeFormat = "hh:mm tt, MMM d";
	}
}
