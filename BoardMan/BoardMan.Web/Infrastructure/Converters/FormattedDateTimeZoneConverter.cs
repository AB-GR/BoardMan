using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace BoardMan.Web.Infrastructure.Converters
{
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
    }
}
