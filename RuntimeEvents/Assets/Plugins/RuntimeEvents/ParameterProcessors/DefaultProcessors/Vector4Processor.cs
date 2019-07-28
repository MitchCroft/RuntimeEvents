using System;

using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide a processor for Vector4 value processing
    /// </summary>
    [CustomTypeProcessor(typeof(Vector4))]
    public sealed class Vector4Processor : AFloatVectorProcessor {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the default value that will be used for Vector4 values
        /// </summary>
        /// <param name="processing">A type value matching the one supplied in the Custom Type Processor Attribute</param>
        /// <returns>Returns a constant zero-ed vector4 value to be used as the default</returns>
        public override object GetDefaultValue(Type processing) { return Vector4.zero; }

        /// <summary>
        /// Get the Vector4 value that is stored within the parameter cache information
        /// </summary>
        /// <param name="parameterCache">The cache object which contains the data that will be extracted for display</param>
        /// <param name="value">The value object that is to be filled with the extracted information</param>
        /// <returns>Returns true if the Vector4 value can be parsed from the supplied value</returns>
        public override bool GetValue(PersistentParameterCache parameterCache, out object value) {
            //Store a default value for the check
            value = null;

            //Check that there is a string to process
            if (string.IsNullOrEmpty(parameterCache.StringValue)) return false;

            //Retrieve the vector for the operation
            value = (Vector4)ExtractVectorValues(parameterCache.StringValue);
            return true;
        }

        /// <summary>
        /// Process the supplied value as a Vector4 for caching
        /// </summary>
        /// <param name="parameterCache">The cache object that will be filled with the data to be stored</param>
        /// <param name="value">The Vector4 value that is to be stored within the cache</param>
        /// <returns>Returns true if the supplied object is a Vector4 value</returns>
        public override bool AssignValue(PersistentParameterCache parameterCache, object value) {
            if (!(value is Vector4)) return false;
            parameterCache.StringValue = SerializeVectorValues((Vector4)value, 4);
            return true;
        }
    }
}