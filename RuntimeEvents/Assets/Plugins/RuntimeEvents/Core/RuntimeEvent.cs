using System;

using UnityEngine;

namespace RuntimeEvents {
    /// <summary>
    /// Basic runtime event that is raised with no parameters
    /// </summary>
    [Serializable] public sealed class RuntimeEvent : RuntimeEventBase {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store a delegate of non-persistent callbacks that should be raised with this event
        /// </summary>
        private RuntimeAction nonPersistentCallbacks;

        /// <summary>
        /// Store a collection of the persistent callbacks that will be raised with the operation
        /// </summary>
        private RuntimeAction persistentCallbacks;

        /// <summary>
        /// Store a combined collection of all of the different callbacks that are to be executed by this event
        /// </summary>
        private RuntimeAction collectiveCallbacks;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Manage subscription to the non-persistent callback functions
        /// </summary>
        public event RuntimeAction Listeners {
            add { AddListener(value); }
            remove { RemoveListener(value); }
        }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise this object with it's base values
        /// </summary>
        public RuntimeEvent() : base() { }

        /// <summary>
        /// Add a non-persistent listener to the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be raised when this event is invoked</param>
        public void AddListener(RuntimeAction call) {
            if (call == null) throw new ArgumentNullException();
            nonPersistentCallbacks += call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove a non-persistent listener from the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be removed from the callback list</param>
        public void RemoveListener(RuntimeAction call) {
            nonPersistentCallbacks -= call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove all non-persistent (i.e. created from script) listeners from the event
        /// </summary>
        /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
        public override void RemoveAllListeners() {
            nonPersistentCallbacks =
            collectiveCallbacks = null;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent)
        /// </summary>
        public void Invoke() {
            //Check if there are any dirty elements
            if (dirtyFlags != 0) {
                //Check the persistent callbacks
                if ((dirtyFlags & EDirtyFlags.Persistent) != 0) {
                    //Clear any pre-existing callbacks
                    persistentCallbacks = null;

                    //Process all existing persistent callbacks
                    if (persistents != null) {
                        for (int i = 0; i < persistents.Length; i++) {
                            //Skip invalid persistent callbacks 
                            if (!persistents[i].IsValid) continue;

                            //Check that this persistent callback can be used
                            switch(persistents[i].EventState) {
                                //If this is off, skip this object
                                case ERuntimeEventState.Off: continue;

                                #if UNITY_EDITOR
                                //Check that the application is running
                                case ERuntimeEventState.RuntimeOnly:
                                    if (!Application.isPlaying) continue;
                                    break;
                                #endif
                            }

                            //Grab the delegate event for this operation
                            RuntimeAction action = PersistentCallbackUtility.CreateDelegateFromPersistent(persistents[i]);

                            //If a callback was received, add it to the collective
                            if (action != null) persistentCallbacks += action;
                        }
                    }
                }

                //Combine all of the available callbacks
                collectiveCallbacks = persistentCallbacks;
                collectiveCallbacks += nonPersistentCallbacks;

                //No longer dirty
                dirtyFlags = 0;
            }

            //Raise the collective callbacks
            if (collectiveCallbacks != null) collectiveCallbacks();
        }
    }

    /// <summary>
    /// Runtime event that is raised with a single parameter
    /// </summary>
    /// <typeparam name="T0">The type of parameter that is supplied to this event when it is invoked</typeparam>
    public class RuntimeEvent<T0> : RuntimeEventBase {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store a delegate of non-persistent callbacks that should be raised with this event
        /// </summary>
        private RuntimeAction<T0> nonPersistentCallbacks;

        /// <summary>
        /// Store a collection of the persistent callbacks that will be raised with the operation
        /// </summary>
        private RuntimeAction<T0> persistentCallbacks;

        /// <summary>
        /// Store a combined collection of all of the different callbacks that are to be executed by this event
        /// </summary>
        private RuntimeAction<T0> collectiveCallbacks;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Manage subscription to the non-persistent callback functions
        /// </summary>
        public event RuntimeAction<T0> Listeners {
            add { AddListener(value); }
            remove { RemoveListener(value); }
        }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise this object with it's base values
        /// </summary>
        public RuntimeEvent() : base(typeof(T0)) { }

        /// <summary>
        /// Add a non-persistent listener to the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be raised when this event is invoked</param>
        public void AddListener(RuntimeAction<T0> call) {
            if (call == null) throw new ArgumentNullException();
            nonPersistentCallbacks += call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove a non-persistent listener from the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be removed from the callback list</param>
        public void RemoveListener(RuntimeAction<T0> call) {
            nonPersistentCallbacks -= call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove all non-persistent (i.e. created from script) listeners from the event
        /// </summary>
        /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
        public override void RemoveAllListeners() {
            nonPersistentCallbacks =
            collectiveCallbacks = null;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent)
        /// </summary>
        /// <param name="param0">The parameter value that is to be passed to the registered callbacks</param>
        public void Invoke(T0 param0) {
            //Check if there are any dirty elements
            if (dirtyFlags != 0) {
                //Check the persistent callbacks
                if ((dirtyFlags & EDirtyFlags.Persistent) != 0) {
                    //Clear any pre-existing callbacks
                    persistentCallbacks = null;

                    //Process all existing persistent callbacks
                    if (persistents != null) {
                        for (int i = 0; i < persistents.Length; i++) {
                            //Skip invalid persistent callbacks 
                            if (!persistents[i].IsValid) continue;

                            //Check that this persistent callback can be used
                            switch (persistents[i].EventState) {
                                //If this is off, skip this object
                                case ERuntimeEventState.Off: continue;

                                #if UNITY_EDITOR
                                //Check that the application is running
                                case ERuntimeEventState.RuntimeOnly:
                                    if (!Application.isPlaying) continue;
                                    break;
                                #endif
                            }

                            //Grab the delegate event for this operation
                            RuntimeAction<T0> action = PersistentCallbackUtility.CreateDelegateFromPersistent<T0>(persistents[i]);

                            //If a callback was received, add it to the collective
                            if (action != null) persistentCallbacks += action;
                        }
                    }
                }

                //Combine all of the available callbacks
                collectiveCallbacks = persistentCallbacks;
                collectiveCallbacks += nonPersistentCallbacks;

                //No longer dirty
                dirtyFlags = 0;
            }

            //Raise the collective callbacks
            if (collectiveCallbacks != null) collectiveCallbacks(param0);
        }
    }

    /// <summary>
    /// Runtime event that is raised with two parameters
    /// </summary>
    /// <typeparam name="T0">The type of the first parameter that is supplied to this event when it is invoked</typeparam>
    /// <typeparam name="T1">The type of the second parameter that is supplied to this event when it is invoked</typeparam>
    public class RuntimeEvent<T0, T1> : RuntimeEventBase {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store a delegate of non-persistent callbacks that should be raised with this event
        /// </summary>
        private RuntimeAction<T0, T1> nonPersistentCallbacks;

        /// <summary>
        /// Store a collection of the persistent callbacks that will be raised with the operation
        /// </summary>
        private RuntimeAction<T0, T1> persistentCallbacks;

        /// <summary>
        /// Store a combined collection of all of the different callbacks that are to be executed by this event
        /// </summary>
        private RuntimeAction<T0, T1> collectiveCallbacks;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Manage subscription to the non-persistent callback functions
        /// </summary>
        public event RuntimeAction<T0, T1> Listeners {
            add { AddListener(value); }
            remove { RemoveListener(value); }
        }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise this object with it's base values
        /// </summary>
        public RuntimeEvent() : base(typeof(T0), typeof(T1)) { }

        /// <summary>
        /// Add a non-persistent listener to the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be raised when this event is invoked</param>
        public void AddListener(RuntimeAction<T0, T1> call) {
            if (call == null) throw new ArgumentNullException();
            nonPersistentCallbacks += call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove a non-persistent listener from the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be removed from the callback list</param>
        public void RemoveListener(RuntimeAction<T0, T1> call) {
            nonPersistentCallbacks -= call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove all non-persistent (i.e. created from script) listeners from the event
        /// </summary>
        /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
        public override void RemoveAllListeners() {
            nonPersistentCallbacks =
            collectiveCallbacks = null;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent)
        /// </summary>
        /// <param name="param0">The first parameter value that is to be passed to the registered callbacks</param>
        /// <param name="param1">The second parameter value that is to be passed to the registered callbacks</param>
        public void Invoke(T0 param0, T1 param1) {
            //Check if there are any dirty elements
            if (dirtyFlags != 0) {
                //Check the persistent callbacks
                if ((dirtyFlags & EDirtyFlags.Persistent) != 0) {
                    //Clear any pre-existing callbacks
                    persistentCallbacks = null;

                    //Process all existing persistent callbacks
                    if (persistents != null) {
                        for (int i = 0; i < persistents.Length; i++) {
                            //Skip invalid persistent callbacks 
                            if (!persistents[i].IsValid) continue;

                            //Check that this persistent callback can be used
                            switch (persistents[i].EventState) {
                                //If this is off, skip this object
                                case ERuntimeEventState.Off: continue;

                                #if UNITY_EDITOR
                                //Check that the application is running
                                case ERuntimeEventState.RuntimeOnly:
                                    if (!Application.isPlaying) continue;
                                    break;
                                #endif
                            }

                            //Grab the delegate event for this operation
                            RuntimeAction<T0, T1> action = PersistentCallbackUtility.CreateDelegateFromPersistent<T0, T1>(persistents[i]);

                            //If a callback was received, add it to the collective
                            if (action != null) persistentCallbacks += action;
                        }
                    }
                }

                //Combine all of the available callbacks
                collectiveCallbacks = persistentCallbacks;
                collectiveCallbacks += nonPersistentCallbacks;

                //No longer dirty
                dirtyFlags = 0;
            }

            //Raise the collective callbacks
            if (collectiveCallbacks != null) collectiveCallbacks(param0, param1);
        }
    }

    /// <summary>
    /// Runtime event that is raised with three parameters
    /// </summary>
    /// <typeparam name="T0">The type of the first parameter that is supplied to this event when it is invoked</typeparam>
    /// <typeparam name="T1">The type of the second parameter that is supplied to this event when it is invoked</typeparam>
    /// <typeparam name="T2">The type of the third parameter that is supplied to this event when it is invoked</typeparam>
    public class RuntimeEvent<T0, T1, T2> : RuntimeEventBase {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store a delegate of non-persistent callbacks that should be raised with this event
        /// </summary>
        private RuntimeAction<T0, T1, T2> nonPersistentCallbacks;

        /// <summary>
        /// Store a collection of the persistent callbacks that will be raised with the operation
        /// </summary>
        private RuntimeAction<T0, T1, T2> persistentCallbacks;

        /// <summary>
        /// Store a combined collection of all of the different callbacks that are to be executed by this event
        /// </summary>
        private RuntimeAction<T0, T1, T2> collectiveCallbacks;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Manage subscription to the non-persistent callback functions
        /// </summary>
        public event RuntimeAction<T0, T1, T2> Listeners {
            add { AddListener(value); }
            remove { RemoveListener(value); }
        }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise this object with it's base values
        /// </summary>
        public RuntimeEvent() : base(typeof(T0), typeof(T1), typeof(T2)) { }

        /// <summary>
        /// Add a non-persistent listener to the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be raised when this event is invoked</param>
        public void AddListener(RuntimeAction<T0, T1, T2> call) {
            if (call == null) throw new ArgumentNullException();
            nonPersistentCallbacks += call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove a non-persistent listener from the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be removed from the callback list</param>
        public void RemoveListener(RuntimeAction<T0, T1, T2> call) {
            nonPersistentCallbacks -= call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove all non-persistent (i.e. created from script) listeners from the event
        /// </summary>
        /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
        public override void RemoveAllListeners() {
            nonPersistentCallbacks =
            collectiveCallbacks = null;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent)
        /// </summary>
        /// <param name="param0">The first parameter value that is to be passed to the registered callbacks</param>
        /// <param name="param1">The second parameter value that is to be passed to the registered callbacks</param>
        /// <param name="param2">The third parameter value that is to be passed to the registered callbacks</param>
        public void Invoke(T0 param0, T1 param1, T2 param2) {
            //Check if there are any dirty elements
            if (dirtyFlags != 0) {
                //Check the persistent callbacks
                if ((dirtyFlags & EDirtyFlags.Persistent) != 0) {
                    //Clear any pre-existing callbacks
                    persistentCallbacks = null;

                    //Process all existing persistent callbacks
                    if (persistents != null) {
                        for (int i = 0; i < persistents.Length; i++) {
                            //Skip invalid persistent callbacks 
                            if (!persistents[i].IsValid) continue;

                            //Check that this persistent callback can be used
                            switch (persistents[i].EventState) {
                                //If this is off, skip this object
                                case ERuntimeEventState.Off: continue;

                                #if UNITY_EDITOR
                                //Check that the application is running
                                case ERuntimeEventState.RuntimeOnly:
                                    if (!Application.isPlaying) continue;
                                    break;
                                #endif
                            }

                            //Grab the delegate event for this operation
                            RuntimeAction<T0, T1, T2> action = PersistentCallbackUtility.CreateDelegateFromPersistent<T0, T1, T2>(persistents[i]);

                            //If a callback was received, add it to the collective
                            if (action != null) persistentCallbacks += action;
                        }
                    }
                }

                //Combine all of the available callbacks
                collectiveCallbacks = persistentCallbacks;
                collectiveCallbacks += nonPersistentCallbacks;

                //No longer dirty
                dirtyFlags = 0;
            }

            //Raise the collective callbacks
            if (collectiveCallbacks != null) collectiveCallbacks(param0, param1, param2);
        }
    }

    /// <summary>
    /// Runtime event that is raised with four parameters
    /// </summary>
    /// <typeparam name="T0">The type of the first parameter that is supplied to this event when it is invoked</typeparam>
    /// <typeparam name="T1">The type of the second parameter that is supplied to this event when it is invoked</typeparam>
    /// <typeparam name="T2">The type of the third parameter that is supplied to this event when it is invoked</typeparam>
    /// <typeparam name="T3">The type of the fourth parameter that is supplied to this event when it is invoked</typeparam>
    public class RuntimeEvent<T0, T1, T2, T3> : RuntimeEventBase {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store a delegate of non-persistent callbacks that should be raised with this event
        /// </summary>
        private RuntimeAction<T0, T1, T2, T3> nonPersistentCallbacks;

        /// <summary>
        /// Store a collection of the persistent callbacks that will be raised with the operation
        /// </summary>
        private RuntimeAction<T0, T1, T2, T3> persistentCallbacks;

        /// <summary>
        /// Store a combined collection of all of the different callbacks that are to be executed by this event
        /// </summary>
        private RuntimeAction<T0, T1, T2, T3> collectiveCallbacks;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Manage subscription to the non-persistent callback functions
        /// </summary>
        public event RuntimeAction<T0, T1, T2, T3> Listeners {
            add { AddListener(value); }
            remove { RemoveListener(value); }
        }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise this object with it's base values
        /// </summary>
        public RuntimeEvent() : base(typeof(T0), typeof(T1), typeof(T2), typeof(T3)) { }

        /// <summary>
        /// Add a non-persistent listener to the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be raised when this event is invoked</param>
        public void AddListener(RuntimeAction<T0, T1, T2, T3> call) {
            if (call == null) throw new ArgumentNullException();
            nonPersistentCallbacks += call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove a non-persistent listener from the RuntimeEvent
        /// </summary>
        /// <param name="call">The function that is to be removed from the callback list</param>
        public void RemoveListener(RuntimeAction<T0, T1, T2, T3> call) {
            nonPersistentCallbacks -= call;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Remove all non-persistent (i.e. created from script) listeners from the event
        /// </summary>
        /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
        public override void RemoveAllListeners() {
            nonPersistentCallbacks =
            collectiveCallbacks = null;
            DirtyNonPersistent();
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent)
        /// </summary>
        /// <param name="param0">The first parameter value that is to be passed to the registered callbacks</param>
        /// <param name="param1">The second parameter value that is to be passed to the registered callbacks</param>
        /// <param name="param2">The third parameter value that is to be passed to the registered callbacks</param>
        /// <param name="param3">The fourth parameter value that is to be passed to the registered callbacks</param>
        public void Invoke(T0 param0, T1 param1, T2 param2, T3 param3) {
            //Check if there are any dirty elements
            if (dirtyFlags != 0) {
                //Check the persistent callbacks
                if ((dirtyFlags & EDirtyFlags.Persistent) != 0) {
                    //Clear any pre-existing callbacks
                    persistentCallbacks = null;

                    //Process all existing persistent callbacks
                    if (persistents != null) {
                        for (int i = 0; i < persistents.Length; i++) {
                            //Skip invalid persistent callbacks 
                            if (!persistents[i].IsValid) continue;

                            //Check that this persistent callback can be used
                            switch (persistents[i].EventState) {
                                //If this is off, skip this object
                                case ERuntimeEventState.Off: continue;

                                #if UNITY_EDITOR
                                //Check that the application is running
                                case ERuntimeEventState.RuntimeOnly:
                                    if (!Application.isPlaying) continue;
                                    break;
                                #endif
                            }

                            //Grab the delegate event for this operation
                            RuntimeAction<T0, T1, T2, T3> action = PersistentCallbackUtility.CreateDelegateFromPersistent<T0, T1, T2, T3>(persistents[i]);

                            //If a callback was received, add it to the collective
                            if (action != null) persistentCallbacks += action;
                        }
                    }
                }

                //Combine all of the available callbacks
                collectiveCallbacks = persistentCallbacks;
                collectiveCallbacks += nonPersistentCallbacks;

                //No longer dirty
                dirtyFlags = 0;
            }

            //Raise the collective callbacks
            if (collectiveCallbacks != null) collectiveCallbacks(param0, param1, param2, param3);
        }
    }
}