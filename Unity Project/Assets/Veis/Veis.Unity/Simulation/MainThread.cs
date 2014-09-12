using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class MainThread
{
    static readonly Queue<Action> _actions = new Queue<Action>();
    private static bool _isInitialised = false;
    private static readonly object _lock = new object();

    public static void Initialise()
    {
        _isInitialised = true;
    }

    public static void QueueAction(Action a)
    {
        if (!_isInitialised)
        {
            return;
        }
        lock (_lock)
        {
            _actions.Enqueue(a);
        }
    }

    public static void DoActions()
    {
        if (!_isInitialised)
        {
            return;
        }
        lock (_lock)
        {
            while (_actions.Count > 0)
            {

                Veis.Data.Logging.Logger.BroadcastMessage(new object(), "Doing action");
                _actions.Dequeue()();
            }
        }
    }
}
