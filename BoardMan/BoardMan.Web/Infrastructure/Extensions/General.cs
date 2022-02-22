namespace BoardMan.Web.Infrastructure.Utils.Extensions
{
	public delegate bool TryParse<TValue>(string raw, out TValue value);

	public static class General
	{
		public static IDictionary<TKey, TValue> AddIfNotNull<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
		{
			if (value == null
				|| (typeof(TValue) == typeof(string)
					&& string.IsNullOrWhiteSpace((string)(object)value)))
				return source;

			source.Add(key, value);
			return source;
		}

		public static bool TryParseValue<TValue>(this IDictionary<string, string> source, string key, TryParse<TValue> tryParse, out TValue value)
			where TValue : struct
		{
			value = default;
			return source.TryGetValue(key, out var raw)
							&& tryParse(raw, out value);
		}
	}
}
