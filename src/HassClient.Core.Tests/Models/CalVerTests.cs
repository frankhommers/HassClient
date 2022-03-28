using System;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models;

[TestFixture(TestOf = typeof(CalVer))]
public class CalVerTests
{
  [Test]
  public void CreateWithNullThrows()
  {
    CalVer version = new();
    Assert.Throws<ArgumentNullException>(() => CalVer.Create(null));
  }

  [Test]
  public void CreateWithInvalidYearThrows()
  {
    CalVer version = new();
    Assert.Throws<ArgumentException>(() => CalVer.Create("invalid"));
  }

  [Test]
  public void CreateWithInvalidMonthThrows()
  {
    CalVer version = new();
    Assert.Throws<ArgumentException>(() => CalVer.Create("2022.invalid"));
  }

  [Test]
  public void CreateWithInvalidMicroAndModifierThrows()
  {
    CalVer version = new();
    Assert.Throws<ArgumentException>(() => CalVer.Create("2022.02.''"));
  }

  [Test]
  public void CreateWithYearAndMonth()
  {
    CalVer version = CalVer.Create("2022.02");

    Assert.AreEqual(2022, version.Year);
    Assert.AreEqual(2, version.Month);
    Assert.AreEqual(0, version.Micro);
    Assert.AreEqual(string.Empty, version.Modifier);
  }

  [Test]
  public void CreateWithYearAndMonthAndMicro()
  {
    CalVer version = CalVer.Create("2022.02.13");

    Assert.AreEqual(2022, version.Year);
    Assert.AreEqual(2, version.Month);
    Assert.AreEqual(13, version.Micro);
    Assert.AreEqual(string.Empty, version.Modifier);
  }

  [Test]
  public void CreateWithYearAndMonthAndModifier()
  {
    CalVer version = CalVer.Create("2022.02.b3");

    Assert.AreEqual(2022, version.Year);
    Assert.AreEqual(2, version.Month);
    Assert.AreEqual(0, version.Micro);
    Assert.AreEqual("b3", version.Modifier);
  }

  [Test]
  public void CreateWithYearAndMonthMicroAndModifier()
  {
    CalVer version = CalVer.Create("2022.02.4b3");

    Assert.AreEqual(2022, version.Year);
    Assert.AreEqual(2, version.Month);
    Assert.AreEqual(4, version.Micro);
    Assert.AreEqual("b3", version.Modifier);
  }

  [Test]
  public void CreateDateIsCorrect()
  {
    CalVer version = CalVer.Create("2022.02.4b3");

    Assert.AreEqual(2022, version.ReleaseDate.Year);
    Assert.AreEqual(2, version.ReleaseDate.Month);
  }

  [Test]
  public void ToStringIsCorrect()
  {
    string expectedVersionString = "2022.2.4b3";
    CalVer version = CalVer.Create(expectedVersionString);

    Assert.AreEqual(expectedVersionString, version.ToString());
  }
}