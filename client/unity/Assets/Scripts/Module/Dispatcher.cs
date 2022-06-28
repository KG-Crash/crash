using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Network;
using Zenject;

namespace Module
{
    public class Dispatcher : MonoBehaviour, IDispatchable
    {
        private readonly Queue<Action> _queue = new Queue<Action>();

        public void Update()
        {
            lock (_queue)
            {
                while (_queue.Count > 0)
                {
                    _queue.Dequeue().Invoke();
                }
            }
        }

        public void Dispatch(IEnumerator action)
        {
            lock (_queue)
            {
                _queue.Enqueue(() =>
                {
                    StartCoroutine(action);
                });
            }
        }

        public void Dispatch(Action action)
        {
            Dispatch(ActionWrapper(action));
        }
        IEnumerator ActionWrapper(Action x)
        {
            x();
            yield return null;
        }
    }
}