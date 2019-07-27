using System;

/// <summary>
/// Used to define an enumeration that can be used as a generic option for selection
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
public sealed class GenericEnumOption : Attribute {}