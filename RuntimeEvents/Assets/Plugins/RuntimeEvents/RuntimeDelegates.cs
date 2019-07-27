/// <summary>
/// Runtime event with no parameters supplied on execution
/// </summary>
public delegate void RuntimeAction();

/// <summary>
/// Runtime event with a single parameter supplied on execution
/// </summary>
/// <typeparam name="T0">The type of the parameter that is supplied</typeparam>
/// <param name="arg0">The value that was supplied on execution of the event</param>
public delegate void RuntimeAction<T0>(T0 arg0);

/// <summary>
/// Runtime event with two parameters supplied on execution
/// </summary>
/// <typeparam name="T0">The type of the first parameter that is supplied</typeparam>
/// <typeparam name="T1">The type of the second parameter that is supplied</typeparam>
/// <param name="arg0">The value that was supplied for the first parameter</param>
/// <param name="arg1">The value that was supplied for the second parameter</param>
public delegate void RuntimeAction<T0, T1>(T0 arg0, T1 arg1);

/// <summary>
/// Runtime event with three parameters supplied on execution
/// </summary>
/// <typeparam name="T0">The type of the first parameter that is supplied</typeparam>
/// <typeparam name="T1">The type of the second parameter that is supplied</typeparam>
/// <typeparam name="T2">The type of the third parameter that is supplied</typeparam>
/// <param name="arg0">The value that was supplied for the first parameter</param>
/// <param name="arg1">The value that was supplied for the second parameter</param>
/// <param name="arg2">The value that was supplied for the third parameter</param>
public delegate void RuntimeAction<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);

/// <summary>
/// Runtime event with four parameters supplied on execution
/// </summary>
/// <typeparam name="T0">The type of the first parameter that is supplied</typeparam>
/// <typeparam name="T1">The type of the second parameter that is supplied</typeparam>
/// <typeparam name="T2">The type of the third parameter that is supplied</typeparam>
/// <typeparam name="T3">The type of the fourth parameter that is supplied</typeparam>
/// <param name="arg0">The value that was supplied for the first parameter</param>
/// <param name="arg1">The value that was supplied for the second parameter</param>
/// <param name="arg2">The value that was supplied for the third parameter</param>
/// <param name="arg3">The value that was supplied for the fourth parameter</param>
public delegate void RuntimeAction<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3);