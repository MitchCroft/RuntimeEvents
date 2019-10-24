using System;
using System.Reflection;

using UnityEngine;

namespace RuntimeEvents {
    /// <summary>
    /// Provide functionality based on setting up persistent callbacks
    /// </summary>
    public static class PersistentCallbackUtility {
        /*----------Variables----------*/
        //PUBLIC

        /// <summary>
        /// Define the search flags that will be used to find callbacks that can be raised on target objects
        /// </summary>
        public const BindingFlags SEARCH_FLAGS = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Retrieve the parameter values required for a persistent callback object
        /// </summary>
        /// <param name="callback">The callback object that will have its parameter information retrieved</param>
        /// <returns>Returns an array of object values that can be passed to a method invoke</returns>
        private static object[] GetPersistentCallbackParameterValues(PersistentCallback callback) {
            //Get the parameter descriptions that are stored in the callback
            var paramsInfo = callback.ParameterInfo;

            //Retrieve the parameter objects that will be raised with the operation
            object[] parameters = new object[paramsInfo != null ? paramsInfo.Length : 0];
            for (int i = 0; i < parameters.Length; i++) {
                //Check that the parameter value is the same
                if (!paramsInfo[i].ParameterType.IsAssignableFrom(paramsInfo[i].ParameterCache.ParameterType)) {
                    Debug.LogErrorFormat("Invalid parameter cache data, parameter type value mismatched for method at index {0}", i);
                    return null;
                }

                //Extract the value that will be used for this entry
                parameters[i] = paramsInfo[i].ParameterCache.Value;
            }
            return parameters;
        }

        //PUBLIC

        /// <summary>
        /// Given the supplied persistent callback object, get the persistent method that can be invoked
        /// </summary>
        /// <param name="callback">The callback object that defines the information for the required callback method</param>
        /// <returns>Returns a MethodInfo object defining the described callback or null if not found</returns>
        public static MethodInfo FindPersistentMethod(PersistentCallback callback) {
            //Check that the target object is valid
            if (!callback.IsValid)
                return null;

            //Retrieve the parameter information that is being used
            var paramaters = callback.ParameterInfo;

            //Find all of the parameter types that are set for the method
            Type[] types = new Type[paramaters != null ? paramaters.Length : 0];
            for (int i = 0; i < types.Length; i++) {
                //Retrieve the type that was found with this value
                Type found = paramaters[i].ParameterType;

                //If no type could be found, no method matches
                if (found == null) return null;
                else types[i] = found;
            }

            //Look for the method with the matching signature
            Type search = callback.Target.GetType();

            //Look for a method that can be used with the set flags
            try { return search.GetMethod(callback.MethodName, SEARCH_FLAGS, null, types, null); }
            catch { return null; }
        }

        /// <summary>
        /// Given the supplied persistent callback object, get the dynamic method that can be invoked
        /// </summary>
        /// <param name="callback">The callback object that defines the information for the required callback method</param>
        /// <param name="parameters">The types that define the parameter elements of the function to find</param>
        /// <returns>Returns a MethodInfo object defining the described callback or null if not found</returns>
        public static MethodInfo FindDynamicMethod(PersistentCallback callback, params Type[] parameters) {
            //Check that the target object is valid
            if (!callback.IsValid)
                return null;

            //Look for the method with the matching signature
            Type search = callback.Target.GetType();

            //Look for a method that can be used with the set flags
            try { return search.GetMethod(callback.MethodName, SEARCH_FLAGS, null, parameters, null); } 
            catch { return null; }
        }

        /// <summary>
        /// Create an invokable function that matches the delegate callback requirements 
        /// </summary>
        /// <param name="callback">The persistent callback object that will be used to create the function</param>
        /// <returns>Returns a delegate event that can be raised or null if unable</returns>
        public static RuntimeAction CreateDelegateFromPersistent(PersistentCallback callback) {
            //Retrieve the method that will be used for the operation
            MethodInfo method = FindPersistentMethod(callback);

            //If there is no method found, can't create a delegate
            if (method == null) return null;

            //Get the parameters for the callback
            object[] parameters = GetPersistentCallbackParameterValues(callback);

            //Check that the parameters could be found
            if (parameters == null) {
                Debug.LogErrorFormat("Unable to generate parameter information, skipping method '{0}' from being raised", method.Name);
                return null;
            }

            //Store the target of the operation
            UnityEngine.Object target = callback.Target;

            //If there are no dynamic parameters and the function is void, bind directly
            if (parameters.Length == 0 && method.ReturnType == typeof(void))
                return (RuntimeAction)Delegate.CreateDelegate(typeof(RuntimeAction), target, method, false);

            //Return the delegate action that will be raised
            else return () => method.Invoke(target, parameters);
        }

        /// <summary>
        /// Create an invokable function that matches the delegate callback requirements
        /// </summary>
        /// <typeparam name="T0">The type of the parameter that will be supplied to the callback operation</typeparam>
        /// <param name="callback">The persistent callback object that will be used to create the function</param>
        /// <returns>Returns a delegate event that can be raised or null if unable</returns>
        public static RuntimeAction<T0> CreateDelegateFromPersistent<T0>(PersistentCallback callback) {
            //If this is a dynamic method, need to relay the received information to the invoke
            if (callback.IsDynamic) {
                //Find the dynamic method to be invoked
                MethodInfo method = FindDynamicMethod(callback, typeof(T0));

                //If there is no method found, can't create a delegate
                if (method == null) return null;

                //Store the target of the operation
                UnityEngine.Object target = callback.Target;

                //If the function is a void return, attempt to map function call directly
                if (method.ReturnType == typeof(void)) 
                    return (RuntimeAction<T0>)Delegate.CreateDelegate(typeof(RuntimeAction<T0>), target, method, false);

                //Otherwise, relay the parameter values
                else return t0 => method.Invoke(target, new object[] { t0 });
            }

            //Otherwise, we can use the persistent stored information
            else {
                //Retrieve the method that will be used for the operation
                MethodInfo method = FindPersistentMethod(callback);

                //If there is no method found, can't create a delegate
                if (method == null) return null;

                //Get the parameters for the callback
                object[] parameters = GetPersistentCallbackParameterValues(callback);

                //Check that the parameters could be found
                if (parameters == null) {
                    Debug.LogErrorFormat("Unable to generate parameter information, skipping method '{0}' from being raised", method.Name);
                    return null;
                }

                //Store the target of the operation
                UnityEngine.Object target = callback.Target;

                //Return the delegate action that will be raised
                return t0 => method.Invoke(target, parameters);
            }
        }

        /// <summary>
        /// Create an invokable function that matches the delegate callback requirements
        /// </summary>
        /// <typeparam name="T0">The type of the first parameter that will be supplied to the callback operation</typeparam>
        /// <typeparam name="T1">The type of the second parameter that will be supplied to the callback operation</typeparam>
        /// <param name="callback">The persistent callback object that will be used to create the function</param>
        /// <returns>Returns a delegate event that can be raised or null if unable</returns>
        public static RuntimeAction<T0, T1> CreateDelegateFromPersistent<T0, T1>(PersistentCallback callback) {
            //If this is a dynamic method, need to relay the received information to the invoke
            if (callback.IsDynamic) {
                //Find the dynamic method to be invoked
                MethodInfo method = FindDynamicMethod(callback, typeof(T0), typeof(T1));

                //If there is no method found, can't create a delegate
                if (method == null) return null;

                //Store the target of the operation
                UnityEngine.Object target = callback.Target;

                //If the function is a void return, attempt to map function call directly
                if (method.ReturnType == typeof(void))
                    return (RuntimeAction<T0, T1>)Delegate.CreateDelegate(typeof(RuntimeAction<T0, T1>), target, method, false);

                //Otherwise, relay the parameter values
                else return (t0, t1) => method.Invoke(target, new object[] { t0, t1 });
            }

            //Otherwise, we can use the persistent stored information
            else {
                //Retrieve the method that will be used for the operation
                MethodInfo method = FindPersistentMethod(callback);

                //If there is no method found, can't create a delegate
                if (method == null) return null;

                //Get the parameters for the callback
                object[] parameters = GetPersistentCallbackParameterValues(callback);

                //Check that the parameters could be found
                if (parameters == null) {
                    Debug.LogErrorFormat("Unable to generate parameter information, skipping method '{0}' from being raised", method.Name);
                    return null;
                }

                //Store the target of the operation
                UnityEngine.Object target = callback.Target;

                //Return the delegate action that will be raised
                return (t0, t1) => method.Invoke(target, parameters);
            }
        }

        /// <summary>
        /// Create an invokable function that matches the delegate callback requirements
        /// </summary>
        /// <typeparam name="T0">The type of the first parameter that will be supplied to the callback operation</typeparam>
        /// <typeparam name="T1">The type of the second parameter that will be supplied to the callback operation</typeparam>
        /// <typeparam name="T2">The type of the third parameter that will be supplied to the callback operation</typeparam>
        /// <param name="callback">The persistent callback object that will be used to create the function</param>
        /// <returns>Returns a delegate event that can be raised or null if unable</returns>
        public static RuntimeAction<T0, T1, T2> CreateDelegateFromPersistent<T0, T1, T2>(PersistentCallback callback) {
            //If this is a dynamic method, need to relay the received information to the invoke
            if (callback.IsDynamic) {
                //Find the dynamic method to be invoked
                MethodInfo method = FindDynamicMethod(callback, typeof(T0), typeof(T1), typeof(T2));

                //If there is no method found, can't create a delegate
                if (method == null) return null;

                //Store the target of the operation
                UnityEngine.Object target = callback.Target;

                //If the function is a void return, attempt to map function call directly
                if (method.ReturnType == typeof(void))
                    return (RuntimeAction<T0, T1, T2>)Delegate.CreateDelegate(typeof(RuntimeAction<T0, T1, T2>), target, method, false);

                //Otherwise, relay the parameter values
                else return (t0, t1, t2) => method.Invoke(target, new object[] { t0, t1, t2 });
            }

            //Otherwise, we can use the persistent stored information
            else {
                //Retrieve the method that will be used for the operation
                MethodInfo method = FindPersistentMethod(callback);

                //If there is no method found, can't create a delegate
                if (method == null) return null;

                //Get the parameters for the callback
                object[] parameters = GetPersistentCallbackParameterValues(callback);

                //Check that the parameters could be found
                if (parameters == null) {
                    Debug.LogErrorFormat("Unable to generate parameter information, skipping method '{0}' from being raised", method.Name);
                    return null;
                }

                //Store the target of the operation
                UnityEngine.Object target = callback.Target;

                //Return the delegate action that will be raised
                return (t0, t1, t2) => method.Invoke(target, parameters);
            }
        }

        /// <summary>
        /// Create an invokable function that matches the delegate callback requirements
        /// </summary>
        /// <typeparam name="T0">The type of the first parameter that will be supplied to the callback operation</typeparam>
        /// <typeparam name="T1">The type of the second parameter that will be supplied to the callback operation</typeparam>
        /// <typeparam name="T2">The type of the third parameter that will be supplied to the callback operation</typeparam>
        /// <typeparam name="T3">The type of the fourth parameter that will be supplied to the callback operation</typeparam>
        /// <param name="callback">The persistent callback object that will be used to create the function</param>
        /// <returns>Returns a delegate event that can be raised or null if unable</returns>
        public static RuntimeAction<T0, T1, T2, T3> CreateDelegateFromPersistent<T0, T1, T2, T3>(PersistentCallback callback) {
            //If this is a dynamic method, need to relay the received information to the invoke
            if (callback.IsDynamic) {
                //Find the dynamic method to be invoked
                MethodInfo method = FindDynamicMethod(callback, typeof(T0), typeof(T1), typeof(T2), typeof(T3));

                //If there is no method found, can't create a delegate
                if (method == null) return null;

                //Store the target of the operation
                UnityEngine.Object target = callback.Target;

                //If the function is a void return, attempt to map function call directly
                if (method.ReturnType == typeof(void))
                    return (RuntimeAction<T0, T1, T2, T3>)Delegate.CreateDelegate(typeof(RuntimeAction<T0, T1, T2, T3>), target, method, false);

                //Otherwise, relay the parameter values
                else return (t0, t1, t2, t3) => method.Invoke(target, new object[] { t0, t1, t2, t3 });
            }

            //Otherwise, we can use the persistent stored information
            else {
                //Retrieve the method that will be used for the operation
                MethodInfo method = FindPersistentMethod(callback);

                //If there is no method found, can't create a delegate
                if (method == null) return null;

                //Get the parameters for the callback
                object[] parameters = GetPersistentCallbackParameterValues(callback);

                //Check that the parameters could be found
                if (parameters == null) {
                    Debug.LogErrorFormat("Unable to generate parameter information, skipping method '{0}' from being raised", method.Name);
                    return null;
                }

                //Store the target of the operation
                UnityEngine.Object target = callback.Target;

                //Return the delegate action that will be raised
                return (t0, t1, t2, t3) => method.Invoke(target, parameters);
            }
        }
    }
}