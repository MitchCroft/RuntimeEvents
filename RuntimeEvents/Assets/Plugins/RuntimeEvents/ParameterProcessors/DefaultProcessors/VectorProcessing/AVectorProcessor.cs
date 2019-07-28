using System;
using System.Text;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Handle the serialization/deserialization process of vector information
    /// </summary>
    public abstract class AVectorProcessor : AParameterProcessor {
        /*----------Variables----------*/
        //CONST

        /// <summary>
        /// Store the array of characters that will be split on when processing serialized vector information
        /// </summary>
        private static readonly char[] SPLIT_CHARS = { '[', ']', ',', ' ' };

        /*----------Functions----------*/
        //PROTECTED

        /// <summary>
        /// Extract the string representations of the vector components from the serialized vector string
        /// </summary>
        /// <param name="serializedVector">The string that contains the vector components to be extracted for parsing</param>
        /// <returns>Returns an array of the vector components that were identified in the string</returns>
        protected string[] ExtractVectorComponents(string serializedVector) {
            if (string.IsNullOrEmpty(serializedVector)) return new string[0];
            else return serializedVector.Split(SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Given the supplied vector components, generate a serializable string
        /// </summary>
        /// <param name="components">An array of the component objects to be added to the vector</param>
        /// <returns>Returns a serializable string containing the string representations of the components/returns>
        protected string SerializeVectorComponents(object[] components) {
            if (components == null) return "[ ]";
            StringBuilder sb = new StringBuilder("[ ");
            for (int i = 0; i < components.Length; i++) {
                sb.Append(components[i]);
                if (i != components.Length - 1) sb.Append(", ");
            }
            sb.Append(" ]");
            return sb.ToString();
        }
    }
}