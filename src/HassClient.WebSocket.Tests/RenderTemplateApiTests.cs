using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class RenderTemplateApiTests : BaseHassWsApiTest
{
  [Test]
  public async Task RenderTemplate()
  {
    string result = await HassWsApi.RenderTemplateAsync("The sun is {{ states('sun.sun') }}");

    Assert.IsNotNull(result);
    Assert.IsTrue(Regex.IsMatch(result, "^The sun is (?:above_horizon|below_horizon)$"));
  }
}