using System;
using System.Linq;
using System.Reflection;
using Bogus;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using HassClient.WebSocket.Messages.Commands.RegistryEntryCollections;
using HassClient.WebSocket.Messages.Response;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class RegistryEntryCollectionCommandProcessor<TFactory, TModel> : BaseCommandProcessor
  where TFactory : RegistryEntryCollectionMessagesFactory<TModel>
  where TModel : RegistryEntryBase
{
  private readonly string _apiPrefix;

  private readonly PropertyInfo _idPropertyInfo;

  private readonly string _modelIdPropertyName;

  protected readonly Faker Faker;

  private bool _isContextReady;

  protected string ModelName;

  public RegistryEntryCollectionCommandProcessor()
    : this(Activator.CreateInstance<TFactory>())
  {
  }

  protected RegistryEntryCollectionCommandProcessor(TFactory factory)
  {
    TFactory modelFactory = factory;
    Faker = new Faker();

    _modelIdPropertyName = $"{modelFactory.ModelName}_id";
    _apiPrefix = modelFactory.ApiPrefix;
    ModelName = modelFactory.ModelName;
    _idPropertyInfo = GetModelIdPropertyInfo();
  }

  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is RawCommandMessage &&
           receivedCommand.Type.StartsWith(_apiPrefix) &&
           IsValidCommandType(receivedCommand.Type);
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    try
    {
      if (!_isContextReady)
      {
        _isContextReady = true;
        PrepareHassContext(context);
      }

      JToken merged = (receivedCommand as RawCommandMessage).MergedObject as JToken;
      string commandType = receivedCommand.Type;
      object result = null;

      if (commandType.EndsWith("list"))
        result = ProccessListCommand(context, merged);
      else if (commandType.EndsWith("create"))
        result = ProccessCreateCommand(context, merged);
      else if (commandType.EndsWith("delete"))
        result = ProccessDeleteCommand(context, merged);
      else if (commandType.EndsWith("update"))
        result = ProccessUpdateCommand(context, merged) ?? ErrorCode.NotFound;
      else
        result = ProccessUnknownCommand(commandType, context, merged);

      if (result is ErrorCode errorCode) return CreateResultMessageWithError(new ErrorInfo(errorCode));

      JRaw resultObject = new(HassSerializer.SerializeObject(result));
      return CreateResultMessageWithResult(resultObject);
    }
    catch (Exception ex)
    {
      return CreateResultMessageWithError(new ErrorInfo(ErrorCode.UnknownError) { Message = ex.Message });
    }
  }

  protected virtual PropertyInfo GetModelIdPropertyInfo()
  {
    Type modelType = typeof(TModel);
    PropertyInfo[] properties = modelType.GetProperties();
    PropertyInfo modelIdProperty =
      properties.FirstOrDefault(x => HassSerializer.GetSerializedPropertyName(x) == _modelIdPropertyName);
    return modelIdProperty ?? properties.Where(x => x.Name.EndsWith("Id")).OrderBy(x => x.Name.Length).FirstOrDefault();
  }

  protected virtual bool IsValidCommandType(string commandType)
  {
    return commandType.EndsWith("create") ||
           commandType.EndsWith("list") ||
           commandType.EndsWith("update") ||
           commandType.EndsWith("delete");
  }

  private string GetModelSerialized(JToken merged)
  {
    string modelSerialized = HassSerializer.SerializeObject(merged);
    string idPropertyName = HassSerializer.GetSerializedPropertyName(_idPropertyInfo);
    if (_modelIdPropertyName != idPropertyName)
      modelSerialized = modelSerialized.Replace(_modelIdPropertyName, idPropertyName);

    return modelSerialized;
  }

  protected virtual TModel DeserializeModel(JToken merged, out string modelSerialized)
  {
    modelSerialized = GetModelSerialized(merged);
    return HassSerializer.DeserializeObject<TModel>(modelSerialized);
  }

  protected virtual void PopulateModel(JToken merged, object target)
  {
    string modelSerialized = GetModelSerialized(merged);
    HassSerializer.PopulateObject(modelSerialized, target);
  }

  protected virtual object ProccessListCommand(MockHassServerRequestContext context, JToken merged)
  {
    return context.HassDb.GetObjects<TModel>();
  }

  protected virtual object ProccessCreateCommand(MockHassServerRequestContext context, JToken merged)
  {
    TModel model = DeserializeModel(merged, out string _);
    _idPropertyInfo.SetValue(model, Faker.RandomUUID());
    context.HassDb.CreateObject(model);
    return model;
  }

  protected virtual object ProccessUpdateCommand(MockHassServerRequestContext context, JToken merged)
  {
    TModel model = DeserializeModel(merged, out string modelSerialized);
    return context.HassDb.UpdateObject(model, new JRaw(modelSerialized));
  }

  protected virtual object ProccessDeleteCommand(MockHassServerRequestContext context, JToken merged)
  {
    TModel model = DeserializeModel(merged, out string _);
    context.HassDb.DeleteObject(model);
    return null;
  }

  protected virtual object ProccessUnknownCommand(string commandType, MockHassServerRequestContext context,
    JToken merged)
  {
    return ErrorCode.NotSupported;
  }

  protected virtual void PrepareHassContext(MockHassServerRequestContext context)
  {
  }
}