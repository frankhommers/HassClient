using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models;
using HassClient.Core.Models.Events;
using HassClient.WebSocket.Messages.Response;

namespace HassClient.WebSocket
{
  /// <summary>
  ///   Helper class to handle state changed event subscriptions from multiple consumers.
  /// </summary>
  public class StateChangedEventListener : IDisposable
  {
    private readonly SemaphoreSlim _refreshSubscriptionsSemaphore = new SemaphoreSlim(0);

    private readonly Dictionary<string, EventHandler<StateChangedEvent>> _stateChangedSubscriptionsByDomain =
      new Dictionary<string, EventHandler<StateChangedEvent>>();

    private readonly Dictionary<string, EventHandler<StateChangedEvent>> _stateChangedSubscriptionsByEntityId =
      new Dictionary<string, EventHandler<StateChangedEvent>>();

    private CancellationTokenSource _cancellationTokenSource;

    private HassClientWebSocket _clientWebSocket;

    private bool _isStateChangedSubscriptionActive;

    private Task _refreshSubscriptionsTask;

    /// <inheritdoc />
    public void Dispose()
    {
      if (_cancellationTokenSource.IsCancellationRequested) return;

      _cancellationTokenSource?.Cancel();
      _cancellationTokenSource.Dispose();
    }

    /// <summary>
    ///   Initialization method of the <see cref="StateChangedEventListener" />.
    /// </summary>
    /// <param name="clientWebSocket">The Home Assistant Web Socket client instance.</param>
    public void Initialize(HassClientWebSocket clientWebSocket)
    {
      if (_clientWebSocket != null)
        throw new InvalidOperationException($"{nameof(StateChangedEventListener)} is already initialized");

      if (clientWebSocket == null) throw new ArgumentNullException(nameof(clientWebSocket));

      _clientWebSocket = clientWebSocket;
      _cancellationTokenSource = new CancellationTokenSource();

      _refreshSubscriptionsTask = Task.Factory.StartNew(
        async () =>
        {
          while (true)
          {
            await _refreshSubscriptionsSemaphore.WaitAsync(_cancellationTokenSource.Token);
            await UpdateStateChangeSockedSubscription(_cancellationTokenSource.Token);
          }
        }, TaskCreationOptions.LongRunning);
    }

    /// <summary>
    ///   Add an <see cref="EventHandler{StateChangedEvent}" /> subscription for an specific entity id.
    /// </summary>
    /// <param name="entityId">The id of the entity to track.</param>
    /// <param name="value">The <see cref="EventHandler{StateChangedEvent}" /> to be included.</param>
    public void SubscribeEntityStateChanged(string entityId, EventHandler<StateChangedEvent> value)
    {
      InternalSubscribeStateChanged(_stateChangedSubscriptionsByEntityId, entityId, value);
    }

    /// <summary>
    ///   Removes an already registered <see cref="EventHandler{StateChangedEvent}" />.
    /// </summary>
    /// <param name="entityId">The id of the tracked entity.</param>
    /// <param name="value">The <see cref="EventHandler{StateChangedEvent}" /> to be removed.</param>
    public void UnsubscribeEntityStateChanged(string entityId, EventHandler<StateChangedEvent> value)
    {
      InternalUnsubscribeStateChanged(_stateChangedSubscriptionsByEntityId, entityId, value);
    }

    /// <summary>
    ///   Add an <see cref="EventHandler{StateChangedEvent}" /> subscription for an specific domain.
    /// </summary>
    /// <param name="domain">The domain to track.</param>
    /// <param name="value">The <see cref="EventHandler{StateChangedEvent}" /> to be included.</param>
    public void SubscribeDomainStateChanged(string domain, EventHandler<StateChangedEvent> value)
    {
      InternalSubscribeStateChanged(_stateChangedSubscriptionsByDomain, domain, value);
    }

    /// <summary>
    ///   Removes an already registered <see cref="EventHandler{StateChangedEvent}" />.
    /// </summary>
    /// <param name="domain">The tracked domain.</param>
    /// <param name="value">The <see cref="EventHandler{StateChangedEvent}" /> to be removed.</param>
    public void UnsubscribeDomainStateChanged(string domain, EventHandler<StateChangedEvent> value)
    {
      InternalUnsubscribeStateChanged(_stateChangedSubscriptionsByDomain, domain, value);
    }

    private void InternalSubscribeStateChanged(Dictionary<string, EventHandler<StateChangedEvent>> register,
      string key, EventHandler<StateChangedEvent> value)
    {
      if (!register.ContainsKey(key))
      {
        register.Add(key, null);

        if (register.Count == 1) _refreshSubscriptionsSemaphore.Release();
      }

      register[key] += value;
    }

    private void InternalUnsubscribeStateChanged(Dictionary<string, EventHandler<StateChangedEvent>> register,
      string key, EventHandler<StateChangedEvent> value)
    {
      if (register.TryGetValue(key, out EventHandler<StateChangedEvent> subscriptions))
      {
        subscriptions -= value;
        if (subscriptions == null)
        {
          register.Remove(key);

          if (register.Count == 0) _refreshSubscriptionsSemaphore.Release();
        }
      }
    }

    private async Task UpdateStateChangeSockedSubscription(CancellationToken cancellationToken)
    {
      bool needsSubcription =
        _stateChangedSubscriptionsByEntityId.Count > 0 || _stateChangedSubscriptionsByDomain.Count > 0;
      bool toggleRequired = _isStateChangedSubscriptionActive ^ needsSubcription;
      if (toggleRequired)
      {
        bool succeed = false;
        if (!_isStateChangedSubscriptionActive)
          succeed = await _clientWebSocket.AddEventHandlerSubscriptionAsync(OnStateChangeEvent,
            KnownEventTypes.StateChanged, cancellationToken);
        else if (_isStateChangedSubscriptionActive)
          succeed = await _clientWebSocket.RemoveEventHandlerSubscriptionAsync(OnStateChangeEvent,
            KnownEventTypes.StateChanged, cancellationToken);

        if (succeed)
        {
          _isStateChangedSubscriptionActive = !_isStateChangedSubscriptionActive;
        }
        else
        {
          // Retry
          _refreshSubscriptionsSemaphore.Release();
          await Task.Delay(100);
        }
      }
    }

    private void OnStateChangeEvent(object sender, EventResultInfo obj)
    {
      StateChangedEvent stateChanged = obj.DeserializeData<StateChangedEvent>();
      if (_stateChangedSubscriptionsByEntityId.TryGetValue(stateChanged.EntityId,
            out EventHandler<StateChangedEvent> eventHandler)) eventHandler.Invoke(this, stateChanged);

      if (_stateChangedSubscriptionsByDomain.TryGetValue(stateChanged.Domain, out eventHandler))
        eventHandler.Invoke(this, stateChanged);
    }
  }
}