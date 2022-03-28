using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models;

[TestFixture(TestOf = typeof(ModifiablePropertyCollection<string>))]
public class ModifiablePropertyCollectionTests
{
  [Test]
  public void DoesNotAcceptsDuplicatedValues()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty();

    const string item = "Test";
    collectionProperty.Value.Add(item);
    collectionProperty.Value.Add(item);

    Assert.That(collectionProperty.Value, Has.Exactly(1).Matches<string>(p => p == item));
  }

  [Test]
  public void AddInvalidValueWhenUsingValidationFuncThrows()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty(0, x => x == "Test");

    Assert.Throws<InvalidOperationException>(() => collectionProperty.Value.Add("Test2"));
  }

  [Test]
  public void AddValidValueWhenUsingValidationFuncDoesNotThrows()
  {
    const string item = "Test";
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty(0, x => x == item);

    Assert.DoesNotThrow(() => collectionProperty.Value.Add(item));
  }

  [Test]
  public void SaveChangesMakesHasPendingChangesFalse()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty(hasChanges: true);
    Assert.IsTrue(collectionProperty.HasPendingChanges);

    collectionProperty.SaveChanges();
    Assert.IsFalse(collectionProperty.HasPendingChanges);
  }

  [Test]
  public void DiscardPendingChangesMakesHasPendingChangesFalse()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty(hasChanges: true);
    Assert.IsTrue(collectionProperty.HasPendingChanges);

    collectionProperty.DiscardPendingChanges();
    Assert.IsFalse(collectionProperty.HasPendingChanges);
  }

  [Test]
  public void DiscardPendingChangesRestoresPreviousValues()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty();
    string[] initialValues = collectionProperty.Value.ToArray();

    collectionProperty.Value.Add("Test");
    collectionProperty.DiscardPendingChanges();

    CollectionAssert.AreEqual(initialValues, collectionProperty.Value);
  }

  [Test]
  public void AddNewValueMakesHasPendingChangesTrue()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty();
    Assert.IsFalse(collectionProperty.HasPendingChanges);

    collectionProperty.Value.Add("Test");

    Assert.IsTrue(collectionProperty.HasPendingChanges);
  }

  [Test]
  public void RemoveValueMakesHasPendingChangesTrue()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty();
    Assert.IsFalse(collectionProperty.HasPendingChanges);

    collectionProperty.Value.Remove(collectionProperty.Value.First());

    Assert.IsTrue(collectionProperty.HasPendingChanges);
  }

  [Test]
  public void ClearValuesMakesHasPendingChangesTrue()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty();
    Assert.IsFalse(collectionProperty.HasPendingChanges);

    collectionProperty.Value.Clear();
    Assert.IsTrue(collectionProperty.HasPendingChanges);
  }

  [Test]
  public void AddAndRemoveValueMakesHasPendingChangesFalse()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty();
    Assert.IsFalse(collectionProperty.HasPendingChanges);

    const string item = "Test";
    collectionProperty.Value.Add(item);
    Assert.IsTrue(collectionProperty.HasPendingChanges);

    collectionProperty.Value.Remove(item);
    Assert.IsFalse(collectionProperty.HasPendingChanges);
  }

  [Test]
  public void RemoveAndAddValueMakesHasPendingChangesFalse()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty();
    string existingValue = collectionProperty.Value.First();
    Assert.IsFalse(collectionProperty.HasPendingChanges);

    collectionProperty.Value.Remove(existingValue);
    Assert.IsTrue(collectionProperty.HasPendingChanges);

    collectionProperty.Value.Add(existingValue);
    Assert.IsFalse(collectionProperty.HasPendingChanges);
  }

  [Test]
  public void ClearAndAddSameValuesMakesHasPendingChangesFalse()
  {
    ModifiablePropertyCollection<string> collectionProperty = CreateCollectionProperty();
    string[] initialValues = collectionProperty.Value.ToArray();
    Assert.IsFalse(collectionProperty.HasPendingChanges);

    collectionProperty.Value.Clear();
    Assert.IsTrue(collectionProperty.HasPendingChanges);

    foreach (string item in initialValues) collectionProperty.Value.Add(item);
    Assert.IsFalse(collectionProperty.HasPendingChanges);
  }

  private ModifiablePropertyCollection<string> CreateCollectionProperty(int elementCount = 2,
    Func<string, bool> validationFunc = null, bool hasChanges = false, [CallerMemberName] string collectionName = null)
  {
    ModifiablePropertyCollection<string> collection = new(collectionName, validationFunc);
    for (int i = 0; i < elementCount; i++) collection.Value.Add(MockHelpers.GetRandomEntityId(KnownDomains.Climate));

    if (!hasChanges) collection.SaveChanges();

    return collection;
  }
}