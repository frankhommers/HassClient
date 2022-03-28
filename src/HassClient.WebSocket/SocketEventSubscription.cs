using System;
using HassClient.WebSocket.Messages.Response;

namespace HassClient.WebSocket
{
  internal class SocketEventSubscription
  {
    private EventHandler<EventResultInfo> _internalEventHandler;

    public SocketEventSubscription(uint subscriptionId)
    {
      SubscriptionId = subscriptionId;
    }

    public uint SubscriptionId { get; set; }

    public uint SubscriptionCount { get; private set; }

    public void AddSubscription(EventHandler<EventResultInfo> eventHandler)
    {
      _internalEventHandler += eventHandler;
      SubscriptionCount++;
    }

    public void RemoveSubscription(EventHandler<EventResultInfo> eventHandler)
    {
      _internalEventHandler -= eventHandler;
      SubscriptionCount--;
    }

    public void Invoke(EventResultInfo eventResultInfo)
    {
      _internalEventHandler?.Invoke(this, eventResultInfo);
    }

    public void ClearAllSubscriptions()
    {
      _internalEventHandler = null;
      SubscriptionCount = 0;
    }
  }
}