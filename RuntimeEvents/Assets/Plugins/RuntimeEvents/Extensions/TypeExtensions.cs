using System;

namespace RuntimeEvents {
    /// <summary>
    /// Provide additional functionality for <see cref="System.Type"/> objects
    /// </summary>
    public static class TypeExtensions {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the first attribute of the specified type
        /// </summary>
        /// <typeparam name="T">The type of attribute to retrieve from the type object</typeparam>
        /// <param name="type">The type object that is to be processed</param>
        /// <param name="inherit">Flag if attributes being searched for should be inherited</param>
        /// <returns>Returns the first instance of T or null if not found</returns>
        public static T GetFirstCustomAttributeOf<T>(this Type type, bool inherit = true) where T : Attribute {
            object[] found = type.GetCustomAttributes(typeof(T), inherit);
            return (found.Length > 0 ? found[0] as T : null);
        }
    }
}