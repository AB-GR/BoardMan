namespace BoardMan.Web.Infrastructure.Utils.Extensions
{
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
	}
}
