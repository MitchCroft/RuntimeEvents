using System;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide a processor for basic boolean value processing
    /// </summary>
    [CustomTypeProcessor(typeof(bool))]
    public sealed class BoolProcessor : AParameterProcessor {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the default value that will be used for boolean values
        /// </summary>
        /// <param name="processing">A type value matching the one supplied in the Custom Type Processor Attribute</param>
        /// <returns>Returns a constant false value to be used as the default</returns>
        public override object GetDefaultValue(Type processing) { return false; }

        /// <summary>
        /// Get the boolean value that is stored within the parameter cache information
        /// </summary>
        /// <param name="parameterCache">The cache object which contains the data that will be extracted for display</param>
        /// <param name="value">The value object that is to be filled with the extracted information</param>
        /// <returns>Simple retrieval operation, always returns true</returns>
        public override bool GetValue(PersistentParameterCache parameterCache, out object value) {
            value = parameterCache.BoolValue;
            return true;
        }

        /// <summary>
        /// Process the supplied value as a boolean for caching
        /// </summary>
        /// <param name="parameterCache">The cache object that will be filled with the data to be stored</param>
        /// <param name="value">The boolean value that is to be stored within the cache</param>
        /// <returns>Returns true if the supplied object is a boolean value</returns>
        public override bool AssignValue(PersistentParameterCache parameterCache, object value) {
            if (!(value is bool)) return false;
            parameterCache.BoolValue = (bool)value;
            return true;
        }
    }
}