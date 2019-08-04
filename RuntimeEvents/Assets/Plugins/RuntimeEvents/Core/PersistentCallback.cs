using System;

using UnityEngine;

namespace RuntimeEvents {
    /// <summary>
    /// Handle the storage of information that is required for raising 
    /// </summary>
    [Serializable] public sealed class PersistentCallback {
        /*----------Types----------*/
        //PUBLIC

        /// <summary>
        /// Store information that is used to determine specific elements required for the raising of persistent callbacks
        /// </summary>
        [Serializable] public sealed class PersistentCallbackParameterInfo {
            /*----------Variables----------*/
            //PRIVATE

            /// <summary>
            /// Store the type of the parameter type that is in use as a string for serialization purposes
            /// </summary>
            [SerializeField] private string parameterType = string.Empty;

            /// <summary>
            /// Store the serialized data that is required for the re-creation of the value that is being sent for processing
            /// </summary>
            [SerializeField] private PersistentParameterCache parameterCache = new PersistentParameterCache();

            /*----------Properties----------*/
            //PUBLIC
            
            /// <summary>
            /// Get and set the parameter type that is assigned to this object
            /// </summary>
            public Type ParameterType {
                get { return (string.IsNullOrEmpty(parameterType) ? null : Type.GetType(parameterType, false)); }
                set { parameterType = (value == null ? string.Empty : PersistentParameterCache.MinifyTypeAssemblyName(value.AssemblyQualifiedName)); }
            }

            /// <summary>
            /// Retrieve the data cache that is stored within the object for the re-creation of value objects
            /// </summary>
            public PersistentParameterCache ParameterCache {
                get { return parameterCache; }
            }
        }

        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store the state that is being used for this event callback
        /// </summary>
        [SerializeField] private ERuntimeEventState eventState;

        /// <summary>
        /// Flags if this event being raised is dynamic and should have its parameter information supplied from the event being called
        /// </summary>
        [SerializeField] private bool isDynamic;

        /// <summary>
        /// Store the target object that will have the functions raised on it
        /// </summary>
        [SerializeField] private UnityEngine.Object target;

        /// <summary>
        /// Store the name of the method that will be invoked in response to events being raised
        /// </summary>
        [SerializeField] private string methodName;

        /// <summary>
        /// Store the parameter information that is required for the raising of the operation
        /// </summary>
        [SerializeField] private PersistentCallbackParameterInfo[] parameterInfo;

        /*----------Properties----------*/
        //PUBLIC
    
        /// <summary>
        /// Checks to see if the current object is valid for use
        /// </summary>
        public bool IsValid { get { return this; } }

        /// <summary>
        /// Get and set the event state that is used for this persistent callback
        /// </summary>
        public ERuntimeEventState EventState {
            get { return eventState; }
            set { eventState = value; }
        }

        /// <summary>
        /// Get and set the flag which indicates if this callback is dynamic (Taking information from the callback)
        /// </summary>
        public bool IsDynamic {
            get { return isDynamic; }
            set { isDynamic = value; }
        }

        /// <summary>
        /// Get and set the target object of the raised persistent operation
        /// </summary>
        public UnityEngine.Object Target {
            get { return target; }
            set {
                target = value;
                methodName = string.Empty;
                parameterInfo = new PersistentCallbackParameterInfo[0];
            }
        }

        /// <summary>
        /// Get and set the method name that is being invoked on method callback
        /// </summary>
        public string MethodName {
            get { return methodName; }
            set { methodName = (string.IsNullOrEmpty(value) ? string.Empty : value); }
        }

        /// <summary>
        /// Get and set the parameter information that is being used to store the value information
        /// </summary>
        public PersistentCallbackParameterInfo[] ParameterInfo {
            get { return parameterInfo; }
            set { parameterInfo = value; }
        }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Force the resetting of base values to start with
        /// </summary>
        public PersistentCallback() { ResetAll(); }

        /// <summary>
        /// Reset the selected method of this callback, clearing assigned data
        /// </summary>
        public void ResetMethod() {
            isDynamic = false;
            methodName = string.Empty;
            parameterInfo = new PersistentCallbackParameterInfo[0];
        }

        /// <summary>
        /// Reset this callback object definition to the default values
        /// </summary>
        public void ResetAll() {
            eventState = ERuntimeEventState.RuntimeOnly;
            target = null;
            ResetMethod();
        }

        /// <summary>
        /// Provide implicit operator checking to see if a persistent callback object is valid for use
        /// </summary>
        /// <param name="callback">The persistent callback object that is being searched</param>
        public static implicit operator bool(PersistentCallback callback) {
            return (callback != null &&                             //Nothing to search
                    callback.target != null &&                      //No target object has been set
                    !string.IsNullOrEmpty(callback.methodName));    //There is no method name assigned
        }
    }

    /// <summary>
    /// Store the different states that a persistent callback can be in
    /// </summary>
    public enum ERuntimeEventState {
        /// <summary>
        /// The function is off and will not be raised on event invoke
        /// </summary>
        Off,

        /// <summary>
        /// The function will be raised when the event is invoked at runtime or within the editor
        /// </summary>
        EditorAndRuntime,

        /// <summary>
        /// The function will be raised only when the application is playing
        /// </summary>
        RuntimeOnly
    }
}