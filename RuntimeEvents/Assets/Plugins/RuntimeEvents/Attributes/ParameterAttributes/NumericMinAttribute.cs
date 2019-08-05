namespace RuntimeEvents {
    /// <summary>
    /// Have the inspector draw the numerical field for an integral or floating point parameter value with a 
    /// defined minimum allowed value
    /// </summary>
    public sealed class NumericMinAttribute : ParameterAttribute {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The minimum value that is allowed to be set via the inspector window
        /// </summary>
        public float Min { get; private set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise the attribute with the minimum allowed value
        /// </summary>
        /// <param name="min">The minimum value that can be used via this parameter</param>
        public NumericMinAttribute(int min) { Min = min; }

        /// <summary>
        /// Initialise the attribute with the minimum allowed value
        /// </summary>
        /// <param name="min">The minimum value that can be used via this parameter</param>
        public NumericMinAttribute(float min) { Min = min; }
    }
}