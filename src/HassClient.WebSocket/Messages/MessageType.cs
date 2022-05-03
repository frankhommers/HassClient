namespace HassClient.WebSocket.Messages
{
  // Extracted from: https://github.com/home-assistant/core/search?q=async_register_command
  internal enum MessageType
  {
    Unknown,

    // Commands
    SubscribeTrigger,
    TestCondition
  }
}