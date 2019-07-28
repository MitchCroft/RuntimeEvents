using System;

using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide a processor for Vector3 value processing
    /// </summary>
    [CustomTypeProcessor(typeof(Vector3))]
    public sealed class Vector3Processor : AFloatVectorProcessor {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the default value that will be used for Vector3 values
        /// </summary>
        /// <param name="processing">A type value matching the one supplied in the Custom Type Processor Attribute</param>
        /// <returns>Returns a constant zero-ed vector3 value to be used as the default</returns>
        public override object GetDefaultValue(Type processing) { return Vector3.zero; }

        /// <summary>
        /// Get the Vector3 value that is stored within the parameter cache information
        /// </summary>
        /// <param name="parameterCache">The cache object which contains the data that will be extracted for display</param>
        /// <param name="value">The value object that is to be filled with the extracted information</param>
        /// <returns>Returns true if the Vector3 value can be parsed from the supplied value</returns>
        public override bool GetValue(PersistentParameterCache parameterCache, out object value) {
            //Store a default value for the check
            value = null;

            //Check that there is a string to process
            if (string.IsNullOrEmpty(parameterCache.StringValue)) return false;

            //Retrieve the vector for the operation
            value = (Vector3)ExtractVectorValues(parameterCache.StringValue);
            return true;
        }

        /// <summary>
        /// Process the supplied value as a Vector3 for caching
        /// </summary>
        /// <param name="parameterCache">The cache object that will be filled with the data to be stored</param>
        /// <param name="value">The Vector3 value that is to be stored within the cache</param>
        /// <returns>Returns true if the supplied object is a Vector3 value</returns>
        public override bool AssignValue(PersistentParameterCache parameterCache, object value) {
            if (!(value is Vector3)) return false;
            parameterCache.StringValue = SerializeVectorValues((Vector3)value, 3);
            return true;
        }
    }
}