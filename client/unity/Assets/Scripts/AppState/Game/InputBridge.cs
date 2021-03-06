using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public interface IInputSubscriber
    {
        void OnPressMainBtn(Vector2 positionSS);
        void OnDragMainBtn(Vector2 positionSS);
        void OnReleaseMainBtn(Vector2 positionSS);
        void OnPressAltBtn(Vector2 positionSS);
        void OnDragAltBtn(Vector2 positionSS);
        void OnReleaseAltBtn(Vector2 positionSS);
        void OnUpKey();
        void OnDownKey();
        void OnLeftKey();
        void OnRightKey();
        void OnScrollDelta(float onScrollDelta);
        void OnAlphaNum(int num);
    }

    public class InputBridge : ScriptableObject
    {
        public static InputBridge _instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void CreateInstance()
        {
            _instance = CreateInstance<InputBridge>();
        }

        private void Awake()
        {
            _inputSubscribers = new List<IInputSubscriber>();
        }

        private List<IInputSubscriber> _inputSubscribers;
        
        public void Register(IInputSubscriber subscriber)
        {
            _inputSubscribers.Add(subscriber);
        }

        public void Unregister(IInputSubscriber subscriber)
        {
            _inputSubscribers.Remove(subscriber);
        }
        
        public void OnPressMainBtn(Vector2 positionSS)
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnPressMainBtn(positionSS);
            }
        }

        public void OnDragMainBtn(Vector2 positionSS)
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnDragMainBtn(positionSS);
            }
        }

        public void OnReleaseMainBtn(Vector2 positionSS)
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnReleaseMainBtn(positionSS);
            }
        }
        
        public void OnPressAltBtn(Vector2 positionSS)
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnPressAltBtn(positionSS);
            }
        }

        public void OnDragAltBtn(Vector2 positionSS)
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnDragAltBtn(positionSS);
            }
        }

        public void OnReleaseAltBtn(Vector2 positionSS)
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnReleaseAltBtn(positionSS);
            }
        }

        public void OnUpKey()
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnUpKey();
            }
        }

        public void OnDownKey()
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnDownKey();
            }
        }

        public void OnLeftKey()
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnLeftKey();
            }
        }

        public void OnRightKey()
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnRightKey();
            }
        }
        
        public void OnScrollDelta(float delta)
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnScrollDelta(delta);
            }
        }
        
        public void OnAlphaNum(int num)
        {
            foreach (var subscriber in _inputSubscribers)
            {
                subscriber.OnAlphaNum(num);
            }
        }
    }
}