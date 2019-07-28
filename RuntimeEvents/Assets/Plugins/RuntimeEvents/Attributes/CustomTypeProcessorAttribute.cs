using System;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Mark a class as a Custom Type Processor that is used to handle the serialization/deserialization of information for caching purposes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CustomTypeProcessorAttribute : Attribute {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The value type that the attached class will be used to process 
        /// </summary>
        public Type ProcessorType { get; private set; }

        /// <summary>
        /// Flags if children of the supplied type should also be handled by this processor
        /// </summary>
        public bool HandleChildren { get; private set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise this object with it's base values
        /// </summary>
        /// <param name="processorType">The type object defining the value type the attached processor is responsible for</param>
        /// <param name="handleChildren">Flags if child classes of the Processor Type should be handled by this processor</param>
        public CustomTypeProcessorAttribute(Type processorType, bool handleChildren = false) { ProcessorType = processorType; HandleChildren = handleChildren; }

        /// <summary>
        /// Override the default behaviour to base the hash code of the attribute off of the stored values
        /// </summary>
        /// <returns>Returns a combined hash to reflect the assigned values</returns>
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 31 + ProcessorType.GetHashCode();
                hash = hash * 31 + HandleChildren.GetHashCode();
                return hash;
            }
        }
    }
}