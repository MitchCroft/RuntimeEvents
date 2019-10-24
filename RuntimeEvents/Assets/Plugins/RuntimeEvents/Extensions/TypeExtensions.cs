using System;
using System.Reflection;

namespace RuntimeEvents {
    /// <summary>
    /// Provide additional functionality to <see cref="System.Type"/> objects
    /// </summary>
    public static class TypeExtensions {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Determine if the supplied Type value has a default constructor
        /// </summary>
        /// <param name="type">The type that is to be checked</param>
        /// <param name="includeNonPublic">Flag that indicates if non-public constructors should also be considered</param>
        /// <returns>Returns true if the type has a default constructor</returns>
        /// <remarks>Implementation from https://stackoverflow.com/a/4681091 </remarks>
        public static bool HasDefaultConstructor(this Type type, bool includeNonPublic = false) {
            //Identify the flags that will be used to search
            BindingFlags flags = (includeNonPublic ?
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic :
                BindingFlags.Instance | BindingFlags.Public
            );

            //Check there is a constructor
            return (type.GetConstructor(flags, null, Type.EmptyTypes, null) != null);
        }

        /// <summary>
        /// Retrieve the default value for the specified type value
        /// </summary>
        /// <param name="type">The type that is to have the default value retrieved</param>
        /// <returns>Returns an object value of the specified type</returns>
        /// <remarks>
        /// Implementation adapted from https://stackoverflow.com/a/353073
        /// </remarks>
        public static object GetDefaultValue(this Type type) {
            return (!type.IsValueType && (typeof(UnityEngine.Object).IsAssignableFrom(type) || !type.HasDefaultConstructor()) ?
                null :
                Activator.CreateInstance(type)
            );
        }
    }
}