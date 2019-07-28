using System;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide a processor for basic Enum value processing
    /// </summary>
    [CustomTypeProcessor(typeof(Enum), true)]
    public sealed class EnumProcessor : AParameterProcessor {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the default value that will be used for Enum values
        /// </summary>
        /// <param name="processing">A type value matching the one supplied in the Custom Type Processor Attribute</param>
        /// <returns>Returns a constant 0 equivelant value to be used as the default</returns>
        public override object GetDefaultValue(Type processing) {
            return Enum.ToObject(processing, 0);
        }

        /// <summary>
        /// Get the Enum that is stored within the parameter cache information
        /// </summary>
        /// <param name="parameterCache">The cache object which contains the data that will be extracted for display</param>
        /// <param name="value">The value object that is to be filled with the extracted information</param>
        /// <returns>Returns true if an enumeration value can be extracted from the cache information</returns>
        public override bool GetValue(PersistentParameterCache parameterCache, out object value) {
            //Set the default value
            value = null;

            //Try to retrieve the enumeration type from the cache
            Type enumType = parameterCache.TypeValue;

            //Check that a type was found
            if (enumType == null || !enumType.IsEnum) return false;

            //Convert the stored value to the type
            value = Enum.ToObject(enumType, parameterCache.IntValue);
            return true;
        }

        /// <summary>
        /// Process the supplied value as an Enumeration for caching
        /// </summary>
        /// <param name="parameterCache">The cache object that will be filled with the data to be stored</param>
        /// <param name="value">The Enumeration value that is to be stored within the cache</param>
        /// <returns>Returns true if the supplied object is a Enumeration value</returns>
        public override bool AssignValue(PersistentParameterCache parameterCache, object value) {
            //Make sure that this type is an enumeration
            if (!(value is Enum)) return false;

            //Store the value in the cache
            parameterCache.IntValue = Convert.ToInt32(value);
            parameterCache.TypeValue = value.GetType();
            return true;
        }
    }
}