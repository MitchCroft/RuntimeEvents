using System;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide a processor for basic Unity Object value processing
    /// </summary>
    [CustomTypeProcessor(typeof(UnityEngine.Object), true)]
    public sealed class UnityObjectProcessor : AParameterProcessor {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the default value that will be used for Unity Object values
        /// </summary>
        /// <param name="processing">The type of Unity Engine Object that is to be processed</param>
        /// <returns>Returns a constant null value to be used as the default</returns>
        public override object GetDefaultValue(Type processing) { return (UnityEngine.Object)null; }

        /// <summary>
        /// Get the Unity Object that is stored within the parameter cache information
        /// </summary>
        /// <param name="parameterCache">The cache object which contains the data that will be extracted for display</param>
        /// <param name="value">The value object that is to be filled with the extracted information</param>
        /// <returns>Simple retrieval operation, always returns true</returns>
        public override bool GetValue(PersistentParameterCache parameterCache, out object value) {
            value = parameterCache.UnityObject;
            return true;
        }

        /// <summary>
        /// Process the supplied value as a Unity Object for caching
        /// </summary>
        /// <param name="parameterCache">The cache object that will be filled with the data to be stored</param>
        /// <param name="value">The Unity Object value that is to be stored within the cache</param>
        /// <returns>Returns true if the supplied object is a Unity Object value</returns>
        public override bool AssignValue(PersistentParameterCache parameterCache, object value) {
            if (typeof(UnityEngine.Object).IsAssignableFrom(value.GetType())) return false;
            parameterCache.UnityObject = (UnityEngine.Object)value;
            parameterCache.TypeValue = (value != null ? value.GetType() : null);
            return true;
        }
    }
}