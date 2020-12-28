using System;
using System.Reflection;
using System.Collections.Generic;

namespace MitchCroft.Utility {
    /// <summary>
    /// Manage the process of traversing the types that are loaded within the active assemblies
    /// </summary>
    public static class AssemblyTypeScanner {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store the array of loaded assemblies within the project for processing
        /// </summary>
        private static readonly Assembly[] LOADED_ASSEMBLIES;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Retrieve all of the loaded assemblies and store them for later processing
        /// </summary>
        static AssemblyTypeScanner() {
            //Get the currently loaded assemblies
            LOADED_ASSEMBLIES = AppDomain.CurrentDomain.GetAssemblies();

            //Active assembly will be the first to be processed
            Assembly current = Assembly.GetExecutingAssembly();
            Array.Sort(LOADED_ASSEMBLIES, (left, right) => (left == current ? -1 : (right == current ? 1 : 0)));
        }

        //PUBLIC

        /// <summary>
        /// Enumerate over all types within the loaded assemblies that meet the specific condition
        /// </summary>
        /// <param name="condition">The condition callback that is used to evaluate if a type should be returned</param>
        /// <returns>Returns an enumerable for all of the types that meet the condition</returns>
        public static IEnumerable<Type> GetTypesWithinAssembly(Func<Type, bool> condition) {
            //Process all of the stored assemblies that are currently loaded
            foreach (Assembly assembly in LOADED_ASSEMBLIES) {
                foreach (Type type in assembly.GetTypes()) {
                    //Check matches the required conditions
                    if (condition(type))
                        yield return type;
                }
            }
        }

        /// <summary>
        /// Enumerate over all types that can be assigned to of the specified type within the loaded assemblies
        /// </summary>
        /// <typeparam name="T">The type that is being looked for within the loaded assemblies</typeparam>
        /// <returns>Returns an enumerable for all types assignable from the specified</returns>
        public static IEnumerable<Type> GetTypesWithinAssembly<T>() {
            //Get the type that is being looked for
            Type search = typeof(T);

            //Process all of the types
            foreach (Assembly assembly in LOADED_ASSEMBLIES) {
                foreach (Type type in assembly.GetTypes()) {
                    //Check if the type can be used
                    if (search.IsAssignableFrom(type))
                        yield return type;
                }
            }
        }
    }
}