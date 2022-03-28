using System;

namespace HassClient.WebSocket.Tests.Mocks;

public class MockEventHandler<T>
{
  public EventHandler<T> EventHandler => Event;
  public event EventHandler<T> Event;
}