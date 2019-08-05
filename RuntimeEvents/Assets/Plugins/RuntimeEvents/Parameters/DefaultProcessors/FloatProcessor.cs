using System;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide a processor for basic integer value processing
    /// </summary>
    [CustomTypeProcessor(typeof(float))]
    public sealed class FloatProcessor : AParameterProcessor {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the default value that will be used for floating point values
        /// </summary>
        /// <param name="processing">A type value matching the one supplied in the Custom Type Processor Attribute</param>
        /// <returns>Returns a constant 0f value to be used as the default</returns>
        public override object GetDefaultValue(Type processing) { return 0f; }

        /// <summary>
        /// Get the floating point value that is stored within the parameter cache information
        /// </summary>
        /// <param name="parameterCache">The cache object which contains the data that will be extracted for display</param>
        /// <param name="value">The value object that is to be filled with the extracted information</param>
        /// <returns>Simple retrieval operation, always returns true</returns>
        public override bool GetValue(PersistentParameterCache parameterCache, out object value) {
            value = parameterCache.FloatValue;
            return true;
        }

        /// <summary>
        /// Process the supplied value as a floating point number for caching
        /// </summary>
        /// <param name="parameterCache">The cache object that will be filled with the data to be stored</param>
        /// <param name="value">The floating point value that is to be stored within the cache</param>
        /// <returns>Returns true if the supplied object is a floating point value</returns>
        public override bool AssignValue(PersistentParameterCache parameterCache, object value) {
            if (!(value is float)) return false;
            parameterCache.FloatValue = (float)value;
            return true;
        }
    }
}