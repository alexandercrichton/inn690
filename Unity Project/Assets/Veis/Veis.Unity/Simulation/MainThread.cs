using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class MainThread
{
    static readonly Queue<Action> _actions = new Queue<Action>();
    private static readonly object _lock = new object();

    public static void QueueAction(Action a)
    {
        lock (_lock)
        {
            _actions.Enqueue(a);
        }
    }

    public static void DoActions()
    {
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
