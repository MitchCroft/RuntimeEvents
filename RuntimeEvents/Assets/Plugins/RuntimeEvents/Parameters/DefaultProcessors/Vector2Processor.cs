using System;

using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide a processor for Vector2 value processing
    /// </summary>
    [CustomTypeProcessor(typeof(Vector2))]
    public sealed class Vector2Processor : AFloatVectorProcessor {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the default value that will be used for Vector2 values
        /// </summary>
        /// <param name="processing">A type value matching the one supplied in the Custom Type Processor Attribute</param>
        /// <returns>Returns a constant zero-ed vector2 value to be used as the default</returns>
        public override object GetDefaultValue(Type processing) { return Vector2.zero; }

        /// <summary>
        /// Get the Vector2 value that is stored within the parameter cache information
        /// </summary>
        /// <param name="parameterCache">The cache object which contains the data that will be extracted for display</param>
        /// <param name="value">The value object that is to be filled with the extracted information</param>
        /// <returns>Returns true if the Vector2 value can be parsed from the supplied value</returns>
        public override bool GetValue(PersistentParameterCache parameterCache, out object value) {
            //Store a default value for the check
            value = null;

            //Check that there is a string to process
            if (string.IsNullOrEmpty(parameterCache.StringValue)) return false;

            //Retrieve the vector for the operation
            value = (Vector2)ExtractVectorValues(parameterCache.StringValue);
            return true;
        }

        /// <summary>
        /// Process the supplied value as a Vector2 for caching
        /// </summary>
        /// <param name="parameterCache">The cache object that will be filled with the data to be stored</param>
        /// <param name="value">The Vector2 value that is to be stored within the cache</param>
        /// <returns>Returns true if the supplied object is a Vector2 value</returns>
        public override bool AssignValue(PersistentParameterCache parameterCache, object value) {
            if (!(value is Vector2)) return false;
            parameterCache.StringValue = SerializeVectorValues((Vector2)value, 2);
            return true;
        }
    }
}