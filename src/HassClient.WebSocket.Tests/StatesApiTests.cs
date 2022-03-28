using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class StatesApiTests : BaseHassWsApiTest
{
  private IEnumerable<StateModel> _states;

  [OneTimeSetUp]
  [Test]
  public async Task GetStates()
  {
    if (_states != null) return;

    _states = await HassWsApi.GetStatesAsync();

    Assert.IsNotNull(_states);
    Assert.IsNotEmpty(_states);
  }

  [Test]
  public void GetStatesHasAttributes()
  {
    Assert.IsTrue(_states.All(x => x.Attributes.Count > 0));
  }

  [Test]
  public void GetStatesHasLastChanged()
  {
    Assert.IsTrue(_states.All(x => x.LastChanged != default));
  }

  [Test]
  public void GetStatesHasLastUpdated()
  {
    Assert.IsTrue(_states.All(x => x.LastUpdated != default));
  }

  [Test]
  public void GetStatesHasState()
  {
    Assert.IsTrue(_states.All(x => x.State != default));
  }

  [Test]
  public void GetStatesHasEntityId()
  {
    Assert.IsTrue(_states.All(x => x.EntityId != default));
  }

  [Test]
  public void GetStatesHasContext()
  {
    Assert.IsTrue(_states.All(x => x.Context != default));
  }
}