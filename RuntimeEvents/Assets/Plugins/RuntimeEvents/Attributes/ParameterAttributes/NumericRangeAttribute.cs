namespace RuntimeEvents {
    /// <summary>
    /// Have the inspector draw with numerical range options for an integral of floating point parameter type
    /// </summary>
    public sealed class NumericRangeAttribute : ParameterAttribute {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The minimum value that is allowed to be set via the inspector window
        /// </summary>
        public float Min { get; private set; }

        /// <summary>
        /// The maximum value that is allowed to be set via the inspector window
        /// </summary>
        public float Max { get; private set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise the attribute with the minimum and maximum bounds
        /// </summary>
        /// <param name="min">The minimum value that can be used via this parameter</param>
        /// <param name="max">The maximum value that can be used via this parameter</param>
        public NumericRangeAttribute(int min, int max) { Min = min; Max = max; }

        /// <summary>
        /// Initialise the attribute with the minimum and maximum bounds
        /// </summary>
        /// <param name="min">The minimum value that can be used via this parameter</param>
        /// <param name="max">The maximum value that can be used via this parameter</param>
        public NumericRangeAttribute(float min, float max) { Min = min; Max = max; }
    }
}