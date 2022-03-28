using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HassClient.Core.Helpers;
using HassClient.Core.Models.KnownEnums;

namespace HassClient.Core.Tests;

public static class MockHelpers
{
  public static string GetRandomEntityId(KnownDomains domain)
  {
    return $"{domain.ToDomainString()}.{DateTime.Now.Ticks}";
  }

  public static string GetRandomTestName([CallerMemberName] string prefix = null)
  {
    return $"{prefix}_{DateTime.Now.Ticks}";
  }

  public static TEnum GetRandom<TEnum>()
    where TEnum : Enum
  {
    return Enum.GetValues(typeof(TEnum))
      .OfType<TEnum>()
      .OrderBy(x => Guid.NewGuid())
      .First();
  }

  public static TEnum GetRandomExcept<TEnum>(params TEnum[] discardedValues)
    where TEnum : Enum
  {
    return Enum.GetValues(typeof(TEnum))
      .OfType<TEnum>()
      .Except(discardedValues)
      .OrderBy(x => Guid.NewGuid())
      .First();
  }
}