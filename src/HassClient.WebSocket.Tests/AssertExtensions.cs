using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class AssertExtensions
{
  public static async Task<T> ThrowsAsync<T>(Task code)
    where T : Exception
  {
    Exception caughtException = null;
    try
    {
      await code;
    }
    catch (Exception e)
    {
      caughtException = e;
    }

    Assert.IsInstanceOf<T>(caughtException);

    return caughtException as T;
  }
}