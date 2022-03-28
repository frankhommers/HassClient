using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WebSocket.Tests.Mocks;

public class MockEventSubscriber
{
  private readonly ConcurrentQueue<object> receivedEventArgs = new();

  public MockEventSubscriber()
  {
    Reset();
  }

  public int HitCount { get; private set; }

  public IEnumerable<object> ReceivedEventArgs => receivedEventArgs;

  public void Reset()
  {
    HitCount = 0;
    receivedEventArgs.Clear();
  }

  public void Handle()
  {
    HitCount++;
  }

  public void Handle<T>(T _)
  {
    Handle();
  }

  public void Handle<T, U>(T _, U u)
  {
    receivedEventArgs.Enqueue(u);
    Handle();
  }

  public async Task<bool> WaitConditionAsync(Func<bool> condition)
  {
    Task<bool> checkTask = Task.Run(async () =>
    {
      while (!condition()) await Task.Delay(25);

      return true;
    });

    return await checkTask;
  }

  public async Task<bool> WaitConditionWithTimeoutAsync(Func<bool> condition, int millisecondsTimeout)
  {
    Task<bool> checkTask = WaitConditionAsync(condition);
    Task waitTask = Task.Delay(millisecondsTimeout);
    await Task.WhenAny(waitTask, checkTask);
    return checkTask.IsCompleted && await checkTask;
  }

  public Task<bool> WaitHitAsync(int minHitsCount = 1)
  {
    return WaitConditionAsync(() => HitCount >= minHitsCount);
  }

  public Task<bool> WaitHitWithTimeoutAsync(int millisecondsTimeout, int minHitsCount = 1)
  {
    return WaitConditionWithTimeoutAsync(() => HitCount >= minHitsCount, millisecondsTimeout);
  }

  public Task<bool> WaitEventArgAsync(object eventArg)
  {
    return WaitConditionAsync(() => ReceivedEventArgs.Contains(eventArg));
  }

  public Task<bool> WaitEventArgWithTimeoutAsync(object eventArg, int millisecondsTimeout)
  {
    return WaitConditionWithTimeoutAsync(() => ReceivedEventArgs.Contains(eventArg), millisecondsTimeout);
  }

  public async Task<T> WaitFirstEventArgAsync<T>(Func<T, bool> predicate)
    where T : class
  {
    T result = default;
    bool aa = await WaitConditionAsync(() =>
    {
      result = ReceivedEventArgs.FirstOrDefault(x => x is T y && predicate(y)) as T;
      return result != null;
    });
    return result;
  }

  public async Task<T> WaitFirstEventArgWithTimeoutAsync<T>(Func<T, bool> predicate, int millisecondsTimeout)
    where T : class
  {
    T result = default;
    bool aa = await WaitConditionWithTimeoutAsync(() =>
    {
      result = ReceivedEventArgs.FirstOrDefault(x => x is T y && predicate(y)) as T;
      return result != null;
    }, millisecondsTimeout);
    return result;
  }

  public Task<T> WaitFirstEventArgAsync<T>()
    where T : class
  {
    return WaitFirstEventArgAsync<T>(x => true);
  }

  public Task<T> WaitFirstEventArgWithTimeoutAsync<T>(int millisecondsTimeout)
    where T : class
  {
    return WaitFirstEventArgWithTimeoutAsync<T>(x => true, millisecondsTimeout);
  }
}