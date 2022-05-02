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

		public static bool IsNullOrEmpty(this Guid? value)
		{
			return value == null || value == Guid.Empty;
		}

		public static EntityUrn ToEntityUrn(this string? entityUrn)
		{				
			if(!string.IsNullOrWhiteSpace(entityUrn) && entityUrn.Split(":").Length == 2 && Guid.TryParse(entityUrn.Split(":")[1], out var entityId))
			{
				return new EntityUrn { EntityId = entityId, EntityName = entityUrn.Split(":")[0] };
			}
			else
			{
				return new EntityUrn();
			}
		}
	}

	public class EntityUrn
	{
		public string? EntityName { get; set; }

		public Guid EntityId { get; set; }
	}
}
