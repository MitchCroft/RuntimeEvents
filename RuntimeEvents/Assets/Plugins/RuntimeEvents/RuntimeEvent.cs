using System;

/// <summary>
/// Basic runtime event that is raised with no parameters
/// </summary>
[Serializable] public sealed class RuntimeEvent : RuntimeEventBase {
    /*----------Variables----------*/
    //PRIVATE

    /// <summary>
    /// Store a delegate of non-persistent callbacks that should be raised with this event
    /// </summary>
    private RuntimeAction nonPersistent;

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
    public RuntimeEvent() : base() {}

    /// <summary>
    /// Add a non-persistent listener to the RuntimeEvent
    /// </summary>
    /// <param name="call">The function that is to be raised when this event is invoked</param>
    public void AddListener(RuntimeAction call) { nonPersistent += call; }

    /// <summary>
    /// Remove a non-persistent listener from the RuntimeEvent
    /// </summary>
    /// <param name="call">The function that is to be removed from the callback list</param>
    public void RemoveListener(RuntimeAction call) { nonPersistent -= call; }

    /// <summary>
    /// Remove all non-persistent (i.e. created from script) listeners from the event
    /// </summary>
    /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
    public override void RemoveAllListeners() { nonPersistent = null; }

    /// <summary>
    /// Invoke all registered callbacks (runtime and persistent)
    /// </summary>
    public void Invoke() {
        InvokePersistentCallbacks();
        if (nonPersistent != null) nonPersistent();
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
    private RuntimeAction<T0> nonPersistent;

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
    public void AddListener(RuntimeAction<T0> call) { nonPersistent += call; }

    /// <summary>
    /// Remove a non-persistent listener from the RuntimeEvent
    /// </summary>
    /// <param name="call">The function that is to be removed from the callback list</param>
    public void RemoveListener(RuntimeAction<T0> call) { nonPersistent -= call; }

    /// <summary>
    /// Remove all non-persistent (i.e. created from script) listeners from the event
    /// </summary>
    /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
    public override void RemoveAllListeners() { nonPersistent = null; }

    /// <summary>
    /// Invoke all registered callbacks (runtime and persistent)
    /// </summary>
    /// <param name="arg0">The parameter value that is to be passed to the registered callbacks</param>
    public void Invoke(T0 arg0) {
        InvokePersistentCallbacks();
        if (nonPersistent != null) nonPersistent(arg0);
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
    private RuntimeAction<T0, T1> nonPersistent;

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
    public void AddListener(RuntimeAction<T0, T1> call) { nonPersistent += call; }

    /// <summary>
    /// Remove a non-persistent listener from the RuntimeEvent
    /// </summary>
    /// <param name="call">The function that is to be removed from the callback list</param>
    public void RemoveListener(RuntimeAction<T0, T1> call) { nonPersistent -= call; }

    /// <summary>
    /// Remove all non-persistent (i.e. created from script) listeners from the event
    /// </summary>
    /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
    public override void RemoveAllListeners() { nonPersistent = null; }

    /// <summary>
    /// Invoke all registered callbacks (runtime and persistent)
    /// </summary>
    /// <param name="arg0">The first parameter value that is to be passed to the registered callbacks</param>
    /// <param name="arg1">The second parameter value that is to be passed to the registered callbacks</param>
    public void Invoke(T0 arg0, T1 arg1) {
        InvokePersistentCallbacks();
        if (nonPersistent != null) nonPersistent(arg0, arg1);
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
    private RuntimeAction<T0, T1, T2> nonPersistent;

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
    public void AddListener(RuntimeAction<T0, T1, T2> call) { nonPersistent += call; }

    /// <summary>
    /// Remove a non-persistent listener from the RuntimeEvent
    /// </summary>
    /// <param name="call">The function that is to be removed from the callback list</param>
    public void RemoveListener(RuntimeAction<T0, T1, T2> call) { nonPersistent -= call; }

    /// <summary>
    /// Remove all non-persistent (i.e. created from script) listeners from the event
    /// </summary>
    /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
    public override void RemoveAllListeners() { nonPersistent = null; }

    /// <summary>
    /// Invoke all registered callbacks (runtime and persistent)
    /// </summary>
    /// <param name="arg0">The first parameter value that is to be passed to the registered callbacks</param>
    /// <param name="arg1">The second parameter value that is to be passed to the registered callbacks</param>
    /// <param name="arg2">The third parameter value that is to be passed to the registered callbacks</param>
    public void Invoke(T0 arg0, T1 arg1, T2 arg2) {
        InvokePersistentCallbacks();
        if (nonPersistent != null) nonPersistent(arg0, arg1, arg2);
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
    private RuntimeAction<T0, T1, T2, T3> nonPersistent;

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
    public void AddListener(RuntimeAction<T0, T1, T2, T3> call) { nonPersistent += call; }

    /// <summary>
    /// Remove a non-persistent listener from the RuntimeEvent
    /// </summary>
    /// <param name="call">The function that is to be removed from the callback list</param>
    public void RemoveListener(RuntimeAction<T0, T1, T2, T3> call) { nonPersistent -= call; }

    /// <summary>
    /// Remove all non-persistent (i.e. created from script) listeners from the event
    /// </summary>
    /// <remarks>All persistent (ie created via inspector) listeners are not affected</remarks>
    public override void RemoveAllListeners() { nonPersistent = null; }

    /// <summary>
    /// Invoke all registered callbacks (runtime and persistent)
    /// </summary>
    /// <param name="arg0">The first parameter value that is to be passed to the registered callbacks</param>
    /// <param name="arg1">The second parameter value that is to be passed to the registered callbacks</param>
    /// <param name="arg2">The third parameter value that is to be passed to the registered callbacks</param>
    /// <param name="arg3">The fourth parameter value that is to be passed to the registered callbacks</param>
    public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3) {
        InvokePersistentCallbacks();
        if (nonPersistent != null) nonPersistent(arg0, arg1, arg2, arg3);
    }
}