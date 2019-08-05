using System;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the base required functions that are needed for setting up and displaying 
    /// persistent parameter options for custom types
    /// </summary>
    public abstract class AParameterProcessor {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the default value that will be used for objects of this type processor
        /// </summary>
        /// <param name="processing">The type of object that is to have a default retrieved (In the event of child processing)</param>
        /// <returns>Returns a value of this processors type</returns>
        public abstract object GetDefaultValue(Type processing);

        /// <summary>
        /// Get the value of this processors type from the values cache
        /// </summary>
        /// <param name="parameterCache">Cache object that stores the information to be parsed into a usable value</param>
        /// <param name="value">The value object to be populated with the parsed information</param>
        /// <returns>Returns true if the value was able to be extracted from the cache</returns>
        public abstract bool GetValue(PersistentParameterCache parameterCache, out object value);

        /// <summary>
        /// Assign a value of this type processor to the supplied cache object
        /// </summary>
        /// <param name="parameterCache">Cache object that is to be filled with the serializable information required to re-create the object value</param>
        /// <param name="value">The value that is to be serialized</param>
        /// <returns>Returns true if the value was able to be serialized to the cache object correctly</returns>
        public abstract bool AssignValue(PersistentParameterCache parameterCache, object value);
    }
}