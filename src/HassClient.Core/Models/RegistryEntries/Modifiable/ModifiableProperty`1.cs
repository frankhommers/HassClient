using System;
using System.Diagnostics.CodeAnalysis;

namespace HassClient.Core.Models.RegistryEntries.Modifiable
{
  /// <summary>
  ///   Represents a modifiable property from a model.
  /// </summary>
  /// <typeparam name="T">The property type.</typeparam>
  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1648:inheritdoc should be used with inheriting class",
    Justification = "Inherits document from base constructor")]
  public class ModifiableProperty<T> : ModifiablePropertyBase<T>
  {
    private T _currentValue;
    private T _unmodifiedValue;

    /// <summary>
    ///   Initializes a new instance of the <see cref="ModifiableProperty{T}" /> class.
    /// </summary>
    /// <inheritdoc />
    public ModifiableProperty(string name)
      : base(name)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ModifiableProperty{T}" /> class.
    /// </summary>
    /// <inheritdoc />
    public ModifiableProperty(string name, Func<T, bool> validationFunc, string validationExceptionMessage = null)
      : base(name, validationFunc, validationExceptionMessage)
    {
    }

    /// <summary>
    ///   Gets or sets a value for the property.
    /// </summary>
    public T Value
    {
      get => _currentValue;
      set
      {
        ValidateValue(value);
        _currentValue = value;
      }
    }

    /// <inheritdoc />
    public override bool HasPendingChanges => !Equals(_currentValue, _unmodifiedValue);

    /// <inheritdoc />
    public override void SaveChanges()
    {
      _unmodifiedValue = _currentValue;
    }

    /// <inheritdoc />
    public override void DiscardPendingChanges()
    {
      _currentValue = _unmodifiedValue;
    }
  }
}