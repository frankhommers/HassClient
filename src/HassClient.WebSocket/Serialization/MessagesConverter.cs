using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Serialization
{
  internal class MessagesConverter : JsonConverter
  {
    private readonly Type baseMessageType = typeof(BaseMessage);

    private readonly Dictionary<string, Func<BaseMessage>> factoriesByType;

    public MessagesConverter()
    {
      factoriesByType = Assembly.GetAssembly(baseMessageType)
        .GetTypes()
        .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(baseMessageType) &&
                    x.GetConstructor(Type.EmptyTypes) != null)
        .Select(x => Expression.Lambda<Func<BaseMessage>>(Expression.New(x)).Compile())
        .ToDictionary(x => x().Type);
    }

    public override bool CanRead => true;

    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType)
    {
      return baseMessageType.IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      JObject obj = JObject.Load(reader);
      string messageType = (string)obj["type"];

      BaseMessage message;
      if (factoriesByType.TryGetValue(messageType, out Func<BaseMessage> factory))
      {
        message = factory();
        serializer.Populate(obj.CreateReader(), message);
      }
      else
      {
        uint id = obj.GetValue("id").Value<uint>();
        obj.Remove("id");
        obj.Remove("type");
        message = new RawCommandMessage(messageType, obj) { Id = id };
      }

      return message;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }
}