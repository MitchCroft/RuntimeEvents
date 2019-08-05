using System;

namespace RuntimeEvents {
    /// <summary>
    /// Specify a method/property that should be excluded from the Runtime Event selection
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OmitRuntimeEventAttribute : Attribute {}
}