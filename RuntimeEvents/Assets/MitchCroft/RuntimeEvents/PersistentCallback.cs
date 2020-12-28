using System;

using MitchCroft.SerializedData;

using UnityEngine;

namespace MitchCroft.RuntimeEvents {
    /// <summary>
    /// Store the information needed to raise a callback from an inspector specified target
    /// </summary>
    [Serializable]
    public sealed class PersistentCallback {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// The state in which this persistent callback event will be raised
        /// </summary>
        [SerializeField] private ERuntimeEventState eventState = ERuntimeEventState.RuntimeOnly;

        /// <summary>
        /// Flags if this event is being raised dynamically and should use the supplied parameter values instead of the persistent data
        /// </summary>
        [SerializeField] private bool isDynamic = false;

        /// <summary>
        /// The serialised reference to the unity object that is to be searched for the event to be raised
        /// </summary>
        [SerializeField] private UnityEngine.Object target = null;

        /// <summary>
        /// The name of the method that is to be raised by this callback
        /// </summary>
        [SerializeField] private string methodName = string.Empty;

        /// <summary>
        /// The parameter value data that is to be supplied for non-dynamic event callbacks
        /// </summary>
        [SerializeField] private SerialData[] parameterInfo = new SerialData[0];
        
        /*----------Events----------*/
        //PUBLIC
    
        
    
        /*----------Properties----------*/
        //PRIVATE
    
        
    
        //PUBLIC
    
        
    
        /*----------Functions----------*/
        //PRIVATE
    
        
    
        //PUBLIC
    
        /// <summary>
        /// Initialise this object with it's base values
        /// </summary>
        public PersistentCallback() {
            
        }

        /// <summary>
        /// Provide implicit operator checking to determine if a persistent callback object is valid for use
        /// </summary>
        /// <param name="callback">The persistent callback object that is being searched</param>
        public static implicit operator bool(PersistentCallback callback) {
            return (callback != null &&                         // There is no object to search
                    callback.target != null &&                  // There is no target object reference to raise the event on
                    !string.IsNullOrEmpty(callback.methodName));// There is no method name assigned to create the callback
        }
    }

    /// <summary>
    /// The different states a persistent callback event can be in for being raised
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
        RuntimeOnly,
    }
}