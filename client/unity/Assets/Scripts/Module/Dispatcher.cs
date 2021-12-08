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
        public static Dispatcher Instance
        {
            get
            {
                if (_ist == null)
                {
                    var obj = new GameObject("Dispatcher");
                    _ist = obj.AddComponent<Dispatcher>();
                    DontDestroyOnLoad(obj);
                }

                return _ist;
            }
        }

        private readonly Queue<Action> _queue = new Queue<Action>();

        void Awake()
        {
            var ist = GetComponent<Dispatcher>();
            if (_ist == null)
            {
                _ist = ist;
                DontDestroyOnLoad(gameObject);
            }
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