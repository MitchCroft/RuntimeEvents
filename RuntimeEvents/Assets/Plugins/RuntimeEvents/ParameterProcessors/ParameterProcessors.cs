using System;
using System.Collections.Generic;

using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Store references to the available parameter processors that will be used for processing persistent callback data
    /// </summary>
    public static class ParameterProcessors {
        /*----------Types----------*/
        //PRIVATE

        /// <summary>
        /// Provide a custom comparer object to be used to differentiate attributes based on their values not their instances
        /// </summary>
        private sealed class CustomTypeProcessorComparer : EqualityComparer<CustomTypeProcessorAttribute> {
            /*----------Functions----------*/
            //PUBLIC

            /// <summary>
            /// Compare two instances of the Custom Type Processor attributes to check if they are equivalent
            /// </summary>
            /// <param name="x">The first instance to be checked</param>
            /// <param name="y">The second instance to be checked</param>
            /// <returns>Returns true if the both of the instances are equivelant</returns>
            public override bool Equals(CustomTypeProcessorAttribute x, CustomTypeProcessorAttribute y) {
                //Check the possible null conditions
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                //Check if the hashes are equivalent
                return (x.GetHashCode() == y.GetHashCode());
            }

            /// <summary>
            /// Wrap the retrieval of the hash code for an attribute
            /// </summary>
            /// <param name="obj">The object that is having its hash code retrieved</param>
            /// <returns>Returns a integer representation of the attributes values</returns>
            public override int GetHashCode(CustomTypeProcessorAttribute obj) { return obj.GetHashCode(); }
        }

        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store a lookup map for the different parameter processor defined objects that exist within the project
        /// </summary>
        /// <remarks>
        /// Mapping for this dictionary is [AttributeMarker, ProcessorClass]
        /// </remarks>
        private static Dictionary<CustomTypeProcessorAttribute, Type> parameterProcessors;

        /// <summary>
        /// Store a cache collection of the instantiated processor objects for each of their respective types
        /// </summary>
        /// <remarks>
        /// Mapping for this dictionary is [AttributeMarker, ProcessorInstance]
        /// </remarks>
        private static Dictionary<CustomTypeProcessorAttribute, AParameterProcessor> cachedProcessors;

        /// <summary>
        /// Store a cached lookup of which processors respond to each type that is tested with this object
        /// </summary>
        /// <remarks>
        /// Mapping for this dictionary is [TestType, Processor], where a null value means no valid entry
        /// </remarks>
        private static Dictionary<Type, AParameterProcessor> typeMapping;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Initialise the object with the available object definitions
        /// </summary>
        static ParameterProcessors() {
            //Create the lookup dictionaries
            CustomTypeProcessorComparer comp = new CustomTypeProcessorComparer();
            parameterProcessors = new Dictionary<CustomTypeProcessorAttribute, Type>(comp);
            cachedProcessors = new Dictionary<CustomTypeProcessorAttribute, AParameterProcessor>(comp);
            typeMapping = new Dictionary<Type, AParameterProcessor>();

            //Find all of the usable processors within the project
            foreach (Type type in AssemblyTypeScanner.GetTypesWithinAssembly<AParameterProcessor>()) {
                //Look for the custom type processor attribute on the object
                CustomTypeProcessorAttribute descriptor = type.GetFirstCustomAttributeOf<CustomTypeProcessorAttribute>(false);

                //If there is no attribute can't use it
                if (descriptor == null) continue;

                //Check if the type has already been used
                if (parameterProcessors.ContainsKey(descriptor))
                    Debug.LogWarningFormat("Class '{0}' is being used to process for Types '{1}' (Include Children: {2}) and will override the processor '{3}'", type.FullName, descriptor.ProcessorType.FullName, descriptor.HandleChildren, parameterProcessors[descriptor].FullName);

                //Save the processor for later use
                parameterProcessors[descriptor] = type;
            }
        }

        /// <summary>
        /// Retrieve a cached processor that can be used for the processing of parameters
        /// </summary>
        /// <param name="descriptor">The descriptor defining the processor to retrieve</param>
        /// <returns>Retrieves an instance of the processor that can be used or null if unable</returns>
        private static AParameterProcessor GetProcessor(CustomTypeProcessorAttribute descriptor) {
            //Make sure that there is a cached instance for this descriptor
            if (!cachedProcessors.ContainsKey(descriptor)) {
                //Try to create an instance with the identified object
                try { cachedProcessors[descriptor] = (AParameterProcessor)Activator.CreateInstance(parameterProcessors[descriptor]); }
                catch (Exception exec) {
                    Debug.LogErrorFormat("Failed to create an instance of the Parameter Processor object '{0}'. Error: {1}", parameterProcessors[descriptor], exec.Message);
                    cachedProcessors[descriptor] = null;
                }
            }

            //Return the cached instance
            return cachedProcessors[descriptor];
        }

        //PUBLIC

        /// <summary>
        /// Check if the specified type has a processor element
        /// </summary>
        /// <param name="type">The type object that marks the parameter processor to check the state of</param>
        /// <returns>Returns true if there is a processor that can be used for the specified type</returns>
        public static bool HasProcessor(Type type) { return GetProcessor(type) != null; }

        /// <summary>
        /// Retrieve the processor for the specified type
        /// </summary>
        /// <param name="type">The type object that marks the parameter processor to retrieve</param>
        /// <returns>Returns a Parameter Processor instance or null if none exists for the supplied type</returns>
        public static AParameterProcessor GetProcessor(Type type) {
            //Check if an existing type mapping value has been saved
            if (!typeMapping.ContainsKey(type)) {
                //Create the exact descriptor required for this type
                CustomTypeProcessorAttribute buffer = new CustomTypeProcessorAttribute(type, false);

                //Check if there is an exact match that can be used
                if (parameterProcessors.ContainsKey(buffer))
                    typeMapping[type] = GetProcessor(buffer);

                //Otherwise, search down the hierarchy chain for a generic option
                else {
                    //Store the type that is being searched for
                    Type search = type;
                    do {
                        //Create the type that is being searched for
                        buffer = new CustomTypeProcessorAttribute(search, true);

                        //Check if there is processor definition for this descriptor
                        if (parameterProcessors.ContainsKey(buffer)) {
                            typeMapping[type] = GetProcessor(buffer);
                            break;
                        }

                        //Advance a step up the hierarchy
                        search = search.BaseType;
                    } while (search != null);

                    //If search is null, nothing could be found. Cache the result
                    if (search == null) typeMapping[type] = null;
                }
            }

            //Return the stored processor
            return typeMapping[type];
        }
    }
}