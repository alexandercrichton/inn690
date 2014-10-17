using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// This class lets you queue generic actions in case they need to be handled by
/// a specific thread. For this simulation that generally means any actions
/// working with Unity types (Transform, etc). Rather than illegally performing
/// those actions on the wrong thread they can be queued here and called
/// by the Unity main thread.
/// </summary>
public static class ThreadedActionHandler
{
    private static readonly Queue<Action> _actions = new Queue<Action>();
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
                _actions.Dequeue()();
            }
        }
    }

    private static void exampleUsage()
    {
        // Using this approach you can queue as many lines as you need.
        // Just be aware that they won't actually be executed until
        // ThreadedActionHandler.DoActions(), which is intended to be
        // called by another thread.
        ThreadedActionHandler.QueueAction(() =>
        {
            GameObject example = (GameObject)GameObject.Find("example");
            example.transform.position = Vector3.zero;
        });
    }
}
