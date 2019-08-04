using System;
using System.Reflection;

namespace RuntimeEvents {
    /// <summary>
    /// Provide additional functionality for <see cref="System.Reflection.ICustomAttributeProvider"/> objects
    /// </summary>
    public static class ICustomAttributeProviderExtensions {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the first attribute of the specified type
        /// </summary>
        /// <typeparam name="T">The type of attribute to retrieve from the type object</typeparam>
        /// <param name="obj">The object that is to be processed</param>
        /// <param name="inherit">Flag if attributes being searched for should be inherited</param>
        /// <returns>Returns the first instance of T or null if not found</returns>
        public static T GetFirstCustomAttributeOf<T>(this ICustomAttributeProvider obj, bool inherit = true) where T : Attribute {
            object[] found = obj.GetCustomAttributes(typeof(T), inherit);
            return (found.Length > 0 ? found[0] as T : null);
        }
    }
}