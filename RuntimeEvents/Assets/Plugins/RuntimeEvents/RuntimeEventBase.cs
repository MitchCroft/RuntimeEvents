using System;
using System.Collections.ObjectModel;

using UnityEngine;

/// <summary>
/// Store the base information that will be used to display the Runtime Event elements within the inspector
/// </summary>
[Serializable] public abstract class RuntimeEventBase {
    /*----------Variables----------*/
    //PRIVATE

    /// <summary>
    /// Store the types that will be used to identify dynamically callable functions for this event
    /// </summary>
    private readonly Type[] DYNAMIC_TYPES;

    //PUBLIC


    /*----------Properties----------*/
    //PRIVATE



    //PUBLIC

    /// <summary>
    /// Retrieve a collection of the dynamic types that are used by this event object
    /// </summary>
    public ReadOnlyCollection<Type> DynamicTypes { get { return Array.AsReadOnly(DYNAMIC_TYPES); } }

    /// <summary>
    /// Get the number of registered persistent listeners
    /// </summary>
    public int PersistentEventCount { get { return GetPersistentEventCount(); } }

    /*----------Functions----------*/
    //PRIVATE



    //PROTECTED

    /// <summary>
    /// Initialise the base object with the definition of the types that 
    /// </summary>
    /// <param name="dynamicTypes">The dynamic types that will be used to populate the inspector with elements that can be dynamically called</param>
    protected RuntimeEventBase(params Type[] dynamicTypes) { DYNAMIC_TYPES = dynamicTypes; }

    /// <summary>
    /// Invoke the persistent callbacks that are stored within this object
    /// </summary>
    /// <param name="parameters">The parameter values that were raised with the event (For Dynamic callbacks))</param>
    protected void InvokePersistentCallbacks(params object[] parameters) {
        //TODO
    }

    //PUBLIC

    /// <summary>
    /// Get the number of registered persistent listeners
    /// </summary>
    /// <returns>Returns the number of persistent listeners</returns>
    public int GetPersistentEventCount() {
        //TODO
        return 0;
    }

    /// <summary>
    /// Get the target method name of the listener at index index
    /// </summary>
    /// <param name="index">Index of the listener to query</param>
    /// <returns>Returns the name of the method that is being raised</returns>
    public string GetPersistentMethodName(int index) {
        //TODO
        return string.Empty;
    }

    /// <summary>
    /// Get the target component of the listener at index index
    /// </summary>
    /// <param name="index">Index of the listener to query</param>
    /// <returns>Returns the object that is the target at the specified index</returns>
    public UnityEngine.Object GetPersistentTarget(int index) {
        //TODO
        return null;
    }

    /// <summary>
    /// Remove all non-persistent listeners that are currently registered with this object
    /// </summary>
    public abstract void RemoveAllListeners();

    /// <summary>
    /// Modify the execution state of a persistent listener
    /// </summary>
    /// <param name="index">Index of the listener to query</param>
    /// <param name="state">State to set</param>
    public void SetPersistentListenerState(int index /*TODO: Add the Call State Options*/) {
        //TODO
    }
}
