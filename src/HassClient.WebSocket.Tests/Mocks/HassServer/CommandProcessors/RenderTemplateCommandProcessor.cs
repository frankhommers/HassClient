using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using HassClient.Core.Helpers;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using HassClient.WebSocket.Messages.Response;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class RenderTemplateCommandProcessor : BaseCommandProcessor
{
  private HashSet<string> _entityIds;

  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is RenderTemplateMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    RenderTemplateMessage commandRenderTemplate = receivedCommand as RenderTemplateMessage;

    _entityIds = new HashSet<string>();
    string result = Regex.Replace(commandRenderTemplate.Template, @"{{ (.*) }}", RenderTemplateValue);
    ListenersTemplateInfo listeners = new()
    {
      All = false,
      Time = false,
      Entities = _entityIds.ToArray()
    };
    listeners.Domains = listeners.Entities.Select(x => x.GetDomain()).ToArray();
    _entityIds = null;

    TemplateEventInfo renderTemplateEvent = new()
    {
      Result = result,
      Listeners = listeners
    };

    EventResultMessage eventMsg = new()
    {
      Id = commandRenderTemplate.Id,
      Event = new JRaw(HassSerializer.SerializeObject(renderTemplateEvent))
    };
    Task.Factory.StartNew(async () =>
    {
      await Task.Delay(40);
      await context.SendMessageAsync(eventMsg, CancellationToken.None);
    });

    return CreateResultMessageWithResult(null);
  }

  private string RenderTemplateValue(Match match)
  {
    string statesPattern = @"states\('(.*)'\)";
    return Regex.Replace(match.Groups[1].Value, statesPattern, RenderStateValue);
  }

  private string RenderStateValue(Match match)
  {
    string entityId = match.Groups[1].Value;
    _entityIds.Add(entityId);

    if (entityId.GetDomain() == "sun")
      return "below_horizon";
    return new Faker().RandomEntityState();
  }
}