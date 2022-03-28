using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HassClient.Core.Models.RegistryEntries.Modifiable
{
  /// <summary>
  ///   Represents a modifiable property from a model.
  /// </summary>
  /// <typeparam name="T">The property type.</typeparam>
  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1648:inheritdoc should be used with inheriting class",
    Justification = "Inherits document from base constructor")]
  public class ModifiablePropertyCollection<T> : ModifiablePropertyBase<T>
  {
    private readonly ObservableHashSet<T> _currentValues;

    /// <summary>
    ///   Initializes a new instance of the <see cref="ModifiablePropertyCollection{T}" /> class.
    /// </summary>
    /// <inheritdoc />
    public ModifiablePropertyCollection(string name, Func<T, bool> validationFunc = null,
      string validationExceptionMessage = null)
      : base(name, validationFunc, validationExceptionMessage)
    {
      _currentValues = new ObservableHashSet<T>(ValidateValue);
    }

    /// <summary>
    ///   Gets property collection values.
    /// </summary>
    public ICollection<T> Value => _currentValues;

    /// <inheritdoc />
    public override bool HasPendingChanges => _currentValues.HasPendingChanges;

    /// <inheritdoc />
    public override void SaveChanges()
    {
      _currentValues.SaveChanges();
    }

    /// <inheritdoc />
    public override void DiscardPendingChanges()
    {
      _currentValues.DiscardPendingChanges();
    }
  }
}