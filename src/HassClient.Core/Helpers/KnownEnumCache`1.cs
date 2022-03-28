using System;
using HassClient.Core.Serialization;

namespace HassClient.Core.Helpers
{
  /// <summary>
  ///   Cache used to reduce use of string in KnownEnum types.
  /// </summary>
  /// <typeparam name="TEnum">The KnownEnum type.</typeparam>
  internal class KnownEnumCache<TEnum>
    where TEnum : struct, Enum
  {
    private readonly Map<string, TEnum> _cache = new Map<string, TEnum>();

    private TEnum? _valueForNullString;

    public KnownEnumCache(TEnum? valueForNullString = null)
    {
      _valueForNullString = valueForNullString;
    }

    public TEnum AsEnum(string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        if (_valueForNullString.HasValue)
          return _valueForNullString.Value;
        throw new ArgumentException($"'{nameof(value)}' cannot be null or empty", nameof(value));
      }

      if (!_cache.Forward.TryGetValue(value, out TEnum result) &&
          HassSerializer.TryGetEnumFromSnakeCase(value, out result))
        _cache.Add(value, result);

      return result;
    }

    public string AsString(TEnum value)
    {
      if (!_cache.Reverse.TryGetValue(value, out string result))
      {
        result = value.ToSnakeCaseUnchecked();
        _cache.Add(result, value);
      }

      return result;
    }
  }
}