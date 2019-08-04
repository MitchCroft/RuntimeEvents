using System;

namespace RuntimeEvents {
    /// <summary>
    /// Define the basis of attributes that can be applied to parameter values to create custom display behaviour in the Inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class ParameterAttribute : Attribute {}
}