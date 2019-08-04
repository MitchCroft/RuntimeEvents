using System;

namespace RuntimeEvents {
    /// <summary>
    /// An attribute that can be used to apply a custom description tooltip within the inspector 
    /// when being displayed as an option within the Runtime Event inspector element
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DescriptionAttribute : Attribute {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The description that has been supplied for the attached element
        /// </summary>
        public string Description { get; private set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise this object with it's base values
        /// </summary>
        /// <param name="description">The description to be displayed for the attached element</param>
        public DescriptionAttribute(string description) { Description = description; }
    }
}