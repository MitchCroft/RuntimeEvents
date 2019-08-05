using System;

using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide a processor for Color value processing
    /// </summary>
    [CustomTypeProcessor(typeof(Color))]
    public sealed class ColorProcessor : AFloatVectorProcessor {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the default value that will be used for Color values
        /// </summary>
        /// <param name="processing">A type value matching the one supplied in the Custom Type Processor Attribute</param>
        /// <returns>Returns a constant white value to be used as the default</returns>
        public override object GetDefaultValue(Type processing) { return Color.white; }

        /// <summary>
        /// Get the Color value that is stored within the parameter cache information
        /// </summary>
        /// <param name="parameterCache">The cache object which contains the data that will be extracted for display</param>
        /// <param name="value">The value object that is to be filled with the extracted information</param>
        /// <returns>Returns true if the Color value can be parsed from the supplied value</returns>
        public override bool GetValue(PersistentParameterCache parameterCache, out object value) {
            //Store a default value for the check
            value = null;

            //Check that there is a string to process
            if (string.IsNullOrEmpty(parameterCache.StringValue)) return false;

            //Retrieve the vector for the operation
            value = (Color)ExtractVectorValues(parameterCache.StringValue);
            return true;
        }

        /// <summary>
        /// Process the supplied value as a Color for caching
        /// </summary>
        /// <param name="parameterCache">The cache object that will be filled with the data to be stored</param>
        /// <param name="value">The Color value that is to be stored within the cache</param>
        /// <returns>Returns true if the supplied object is a Color value</returns>
        public override bool AssignValue(PersistentParameterCache parameterCache, object value) {
            if (!(value is Color)) return false;
            parameterCache.StringValue = SerializeVectorValues((Color)value, 4);
            return true;
        }
    }
}