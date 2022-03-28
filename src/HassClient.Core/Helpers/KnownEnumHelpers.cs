using System;
using HassClient.Core.Models.Events;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Serialization;

namespace HassClient.Core.Helpers
{
  /// <summary>
  ///   Contains extension methods to convert between known enums such as <see cref="KnownDomains" />,
  ///   <see cref="KnownServices" /> or <see cref="KnownEventTypes" /> to snake case strings.
  ///   <para>
  ///     It uses an internal cache, so these methods should be used instead of
  ///     <see cref="HassSerializer.ToSnakeCase{TEnum}" />.
  ///   </para>
  /// </summary>
  public static class KnownEnumHelpers
  {
    private static readonly KnownEnumCache<KnownDomains> KnownDomainsCache = new KnownEnumCache<KnownDomains>();

    private static readonly KnownEnumCache<KnownEventTypes>
      KnownEventTypesCache = new KnownEnumCache<KnownEventTypes>();

    private static readonly KnownEnumCache<KnownServices> KnownServicesCache = new KnownEnumCache<KnownServices>();

    private static readonly KnownEnumCache<KnownStates> KnownStatesCache =
      new KnownEnumCache<KnownStates>(KnownStates.Unknown);

    /// <summary>
    ///   Converts a given <paramref name="domain" /> to <see cref="KnownDomains" />.
    /// </summary>
    /// <param name="domain">The domain. (e.g. <c>light</c>).</param>
    /// <returns>
    ///   The domain as a <see cref="KnownDomains" /> if defined; otherwise, <see cref="KnownDomains.Undefined" />.
    /// </returns>
    public static KnownDomains AsKnownDomain(this string domain)
    {
      if (string.IsNullOrEmpty(domain))
        throw new ArgumentException($"'{nameof(domain)}' cannot be null or empty", nameof(domain));

      return KnownDomainsCache.AsEnum(domain);
    }

    /// <summary>
    ///   Converts a given <see cref="KnownDomains" /> to a snake case <see cref="string" />.
    /// </summary>
    /// <param name="domain">A <see cref="KnownDomains" />.</param>
    /// <returns>
    ///   The domain as a <see cref="string" />.
    /// </returns>
    public static string ToDomainString(this KnownDomains domain)
    {
      return KnownDomainsCache.AsString(domain);
    }

    /// <summary>
    ///   Converts a given snake case <paramref name="eventType" /> to <see cref="KnownDomains" />.
    /// </summary>
    /// <param name="eventType">
    ///   The event type as a snake case <see cref="string" />. (e.g. <c>state_changed</c>).
    /// </param>
    /// <returns>
    ///   The event type as a <see cref="KnownEventTypes" /> if defined; otherwise, <see cref="KnownEventTypes.Any" />.
    /// </returns>
    public static KnownEventTypes AsKnownEventType(this string eventType)
    {
      if (string.IsNullOrEmpty(eventType))
        throw new ArgumentException($"'{nameof(eventType)}' cannot be null or empty", nameof(eventType));

      return KnownEventTypesCache.AsEnum(eventType);
    }

    /// <summary>
    ///   Converts a given <see cref="KnownEventTypes" /> to a snake case <see cref="string" />.
    /// </summary>
    /// <param name="eventType">A <see cref="KnownEventTypes" />.</param>
    /// <returns>
    ///   The service as a <see cref="string" />.
    /// </returns>
    public static string ToEventTypeString(this KnownEventTypes eventType)
    {
      return KnownEventTypesCache.AsString(eventType);
    }

    /// <summary>
    ///   Converts a given snake case <paramref name="service" /> to <see cref="KnownServices" />.
    /// </summary>
    /// <param name="service">
    ///   The service as a snake case <see cref="string" />. (e.g. <c>turn_on</c>).
    /// </param>
    /// <returns>
    ///   The service as a <see cref="KnownServices" /> if defined; otherwise, <see cref="KnownServices.Undefined" />.
    /// </returns>
    public static KnownServices AsKnownService(this string service)
    {
      if (string.IsNullOrEmpty(service))
        throw new ArgumentException($"'{nameof(service)}' cannot be null or empty", nameof(service));

      return KnownServicesCache.AsEnum(service);
    }

    /// <summary>
    ///   Converts a given <see cref="KnownServices" /> to a snake case <see cref="string" />.
    /// </summary>
    /// <param name="service">A <see cref="KnownServices" />.</param>
    /// <returns>
    ///   The service as a <see cref="string" />.
    /// </returns>
    public static string ToServiceString(this KnownServices service)
    {
      return KnownServicesCache.AsString(service);
    }

    /// <summary>
    ///   Converts a given snake case <paramref name="state" /> to <see cref="KnownStates" />.
    /// </summary>
    /// <param name="state">
    ///   The state as a snake case <see cref="string" />. (e.g. <c>above_horizon</c>).
    /// </param>
    /// <returns>
    ///   The state as a <see cref="KnownStates" /> if defined; otherwise, <see cref="KnownStates.Undefined" />.
    /// </returns>
    public static KnownStates AsKnownState(this string state)
    {
      return KnownStatesCache.AsEnum(state);
    }

    /// <summary>
    ///   Converts a given <see cref="KnownStates" /> to a snake case <see cref="string" />.
    /// </summary>
    /// <param name="state">A <see cref="KnownStates" />.</param>
    /// <returns>
    ///   The state as a <see cref="string" />.
    /// </returns>
    public static string ToStateString(this KnownStates state)
    {
      return KnownStatesCache.AsString(state);
    }
  }
}