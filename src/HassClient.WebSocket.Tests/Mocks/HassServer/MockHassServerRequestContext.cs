using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands.RegistryEntryCollections;
using HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

namespace HassClient.WebSocket.Tests.Mocks.HassServer;

public class MockHassServerRequestContext
{
  private const int INCOMING_BUFFER_SIZE = 4 * 1024 * 1024; // 4MB

  private readonly List<BaseCommandProcessor> _commandProcessors;

  public readonly EventSubscriptionsProcessor EventSubscriptionsProcessor;

  public readonly MockHassDb HassDb;

  private readonly ArraySegment<byte> _receivingBuffer;

  public MockHassServerRequestContext(MockHassDb hassDb, System.Net.WebSockets.WebSocket webSocket)
  {
    IsAuthenticating = true;
    LastReceivedID = 0;
    HassDb = hassDb;
    WebSocket = webSocket;
    _receivingBuffer = new ArraySegment<byte>(new byte[INCOMING_BUFFER_SIZE]);
    EventSubscriptionsProcessor = new EventSubscriptionsProcessor();
    _commandProcessors = new List<BaseCommandProcessor>
    {
      EventSubscriptionsProcessor,
      new PingCommandProcessor(),
      new GetConfigurationCommandProcessor(),
      new EntitySourceCommandProcessor(),
      new PanelsCommandProcessor(),
      new RenderTemplateCommandProcessor(),
      new SearchCommandProcessor(),
      new CallServiceCommandProcessor(),
      new GetServicesCommandProcessor(),
      new GetStatesCommandProcessor(),
      new RegistryEntryCollectionCommandProcessor<AreaRegistryMessagesFactory, Area>(),
      new DeviceStorageCollectionCommandProcessor(),
      new UserStorageCollectionCommandProcessor(),
      new EntityRegistryStorageCollectionCommandProcessor(),
      new StorageCollectionCommandProcessor<InputBoolean>(),
      new StorageCollectionCommandProcessor<Zone>()
    };
  }

  public bool IsAuthenticating { get; set; }
  public uint LastReceivedID { get; set; }

  public System.Net.WebSockets.WebSocket WebSocket { get; }

  public bool TryProccesMessage(BaseIdentifiableMessage receivedCommand, out BaseIdentifiableMessage result)
  {
    BaseCommandProcessor processor = _commandProcessors.FirstOrDefault(x => x.CanProcess(receivedCommand));
    if (processor == null)
    {
      Trace.WriteLine($"[MockHassServer] No Command processor found for received message '{receivedCommand.Type}'");
      result = null;
      return false;
    }

    result = processor.ProcessCommand(this, receivedCommand);
    return true;
  }

  public async Task<TMessage> ReceiveMessageAsync<TMessage>(CancellationToken cancellationToken)
    where TMessage : BaseMessage
  {
    StringBuilder receivedString = new();
    WebSocketReceiveResult rcvResult;
    do
    {
      rcvResult = await WebSocket.ReceiveAsync(_receivingBuffer, cancellationToken);
      byte[] msgBytes = _receivingBuffer.Skip(_receivingBuffer.Offset).Take(rcvResult.Count).ToArray();
      receivedString.Append(Encoding.UTF8.GetString(msgBytes));
    } while (!rcvResult.EndOfMessage);

    string rcvMsg = receivedString.ToString();
    return HassSerializer.DeserializeObject<TMessage>(rcvMsg);
  }

  public async Task SendMessageAsync(BaseMessage message, CancellationToken cancellationToken)
  {
    string sendMsg = HassSerializer.SerializeObject(message);
    byte[] sendBytes = Encoding.UTF8.GetBytes(sendMsg);
    ArraySegment<byte> sendBuffer = new(sendBytes);
    await WebSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, true, cancellationToken);
  }
}