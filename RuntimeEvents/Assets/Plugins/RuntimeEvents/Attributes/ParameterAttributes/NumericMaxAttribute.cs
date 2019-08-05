namespace RuntimeEvents {
    /// <summary>
    /// Have the inspector draw the numerical field for an integral or floating point parameter value with a
    /// defined maximum allowed value
    /// </summary>
    public sealed class NumericMaxAttribute : ParameterAttribute {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The maximum value that is allowed to be set via the inspector window
        /// </summary>
        public float Max { get; private set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise the attribute with the minimum and maximum bounds
        /// </summary>
        /// <param name="max">The maximum value that can be used via this parameter</param>
        public NumericMaxAttribute(int max) { Max = max; }

        /// <summary>
        /// Initialise the attribute with the minimum and maximum bounds
        /// </summary>
        /// <param name="max">The maximum value that can be used via this parameter</param>
        public NumericMaxAttribute(float max) { Max = max; }
    }
}