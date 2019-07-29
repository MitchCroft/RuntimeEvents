using System;
using System.Collections.ObjectModel;

using UnityEngine;

namespace RuntimeEvents {
    /// <summary>
    /// Store the base information that will be used to display the Runtime Event elements within the inspector
    /// </summary>
    [Serializable]
    public abstract class RuntimeEventBase {
        /*----------Types----------*/
        //PROTECTED

        /// <summary>
        /// Identify flags that can be used to force updates of the cached delegate actions
        /// </summary>
        [Flags] protected enum EDirtyFlags : byte {
            //Individual
            Persistent      = 1 << 0,
            NonPersistent   = 1 << 1,

            //Collections
            All             = Persistent | NonPersistent
        }

        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store the types that will be used to identify dynamically callable functions for this event
        /// </summary>
        private readonly Type[] DYNAMIC_TYPES;

        /// <summary>
        /// Store a collection of persistent callbacks that can be invoked during execution
        /// </summary>
        [SerializeField] protected PersistentCallback[] persistents;

        /// <summary>
        /// Store flags that are used to indicate specific contained elements that are in need of regenerating
        /// </summary>
        protected EDirtyFlags dirtyFlags = EDirtyFlags.All;

        /*----------Properties----------*/
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
        //PROTECTED

        /// <summary>
        /// Initialise the base object with the definition of the types that 
        /// </summary>
        /// <param name="dynamicTypes">The dynamic types that will be used to populate the inspector with elements that can be dynamically called</param>
        protected RuntimeEventBase(params Type[] dynamicTypes) { DYNAMIC_TYPES = dynamicTypes; }

        //PUBLIC

        /// <summary>
        /// Get the number of registered persistent listeners
        /// </summary>
        /// <returns>Returns the number of persistent listeners</returns>
        public int GetPersistentEventCount() { return (persistents != null ? persistents.Length : 0); }

        /// <summary>
        /// Get the target method name of the listener at index index
        /// </summary>
        /// <param name="index">Index of the listener to query</param>
        /// <returns>Returns the name of the method that is being raised</returns>
        public string GetPersistentMethodName(int index) {
            if (persistents == null || index < 0 || index >= persistents.Length)
                throw new IndexOutOfRangeException("Index out of stored Persistents object range");
            return persistents[index].MethodName;
        }

        /// <summary>
        /// Get the target component of the listener at index index
        /// </summary>
        /// <param name="index">Index of the listener to query</param>
        /// <returns>Returns the object that is the target at the specified index</returns>
        public UnityEngine.Object GetPersistentTarget(int index) {
            if (persistents == null || index < 0 || index >= persistents.Length)
                throw new IndexOutOfRangeException("Index out of stored Persistents object range");
            return persistents[index].Target;
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
        public void SetPersistentListenerState(int index, ERuntimeEventState state) {
            if (persistents == null || index < 0 || index >= persistents.Length)
                throw new IndexOutOfRangeException("Index out of stored Persistents object range");
            if (persistents[index].EventState != state) {
                persistents[index].EventState = state;
                DirtyPersistent();
            }
        }

        /// <summary>
        /// Flag that the persistent callbacks are in need of regenerating
        /// </summary>
        public void DirtyPersistent() { dirtyFlags |= EDirtyFlags.Persistent; }

        /// <summary>
        /// Flag that non-persistent callbacks are in need of regenerating
        /// </summary>
        public void DirtyNonPersistent() { dirtyFlags |= EDirtyFlags.NonPersistent; }

        /// <summary>
        /// Flag all callbacks are in need of regenerating
        /// </summary>
        public void DirtAll() { dirtyFlags = EDirtyFlags.All; }
    }
}