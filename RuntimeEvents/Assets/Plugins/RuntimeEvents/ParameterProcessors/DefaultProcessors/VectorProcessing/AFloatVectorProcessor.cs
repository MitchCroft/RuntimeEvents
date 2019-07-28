using System;

using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide a base point for processing vectors of float information
    /// </summary>
    public abstract class AFloatVectorProcessor : AVectorProcessor {
        /*----------Variables----------*/
        //CONST

        /// <summary>
        /// Store the maximum number of components that can be extracted from a vector processor
        /// </summary>
        protected const int MAXIMUM_COMPONENTS = 4;

        /*----------Functions----------*/
        //PROTECTED

        /// <summary>
        /// Retrieve the vector components deserialized from the supplied string
        /// </summary>
        /// <param name="serializedVector">The serialized string that contains the information to be parsed</param>
        /// <returns>Returns a Vector4 with the deserialized vector information</returns>
        protected Vector4 ExtractVectorValues(string serializedVector) {
            //Store the vector to fill with information
            Vector4 vec = Vector4.zero;

            //Retrieve the component elements of the vector
            string[] comps = ExtractVectorComponents(serializedVector);

            //Fill the vector with the information
            float buffer;
            for (int i = 0; i < comps.Length && i < MAXIMUM_COMPONENTS; i++)
                vec[i] = (float.TryParse(comps[i], out buffer) ? buffer : 0f);
            return vec;
        }

        /// <summary>
        /// Serialize the supplied vector into a store-able string value
        /// </summary>
        /// <param name="vector">The vector object containing the data to be serialized</param>
        /// <param name="count">The number of components to take from the vector</param>
        /// <returns>Returns a string of the serialized vector elements</returns>
        protected string SerializeVectorValues(Vector4 vector, int count) {
            //Store all of the components within the array
            object[] components = new object[Math.Min(count, MAXIMUM_COMPONENTS)];
            for (int i = 0; i < components.Length; i++)
                components[i] = vector[i];

            //Serialize the components
            return SerializeVectorComponents(components);
        }
    }
}