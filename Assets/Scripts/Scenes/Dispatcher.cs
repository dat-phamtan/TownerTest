using System;
using System.Collections.Generic;
using UnityEngine;

public class Dispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _queue = new();
    private static readonly object _lock = new();

    public static void Enqueue(Action action)
    {
        lock (_lock) 
            _queue.Enqueue(action);
    }

    void Update()
    {
        lock( _lock)
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue()?.Invoke();
            }
        }
    }
}
