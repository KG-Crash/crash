using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Network;

namespace Module
{
    public class Dispatcher : MonoBehaviour, IDispatchable
    {
        private static Dispatcher _ist = null;
        private readonly Queue<Action> _queue = new Queue<Action>();
        public static Dispatcher Instance => _ist;

        void Awake()
        {
            var inst = GetComponent<Dispatcher>();
            if (_ist == null)
            {
                _ist = inst;
                DontDestroyOnLoad(gameObject);
            }
            else if (_ist != inst)
                Destroy(gameObject);
        }

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