using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(GameController), typeof(GameUI))]
    public class GameInput : MonoBehaviour, IInputSubscriber
    {
        private GameController _controller;
        private GameUI _ui;
        
        private void Awake()
        {
            _controller = GetComponent<GameController>();
            _ui = GetComponent<GameUI>();
            
            InputBridge._instance.Register(this);
        }

        private void OnDestroy()
        {
            InputBridge._instance.Unregister(this);
        }

        public void OnPressMainBtn(Vector2 positionSS)
        {
        }

        public void OnDragMainBtn(Vector2 positionSS)
        {
        }
        public void OnReleaseMainBtn(Vector2 positionSS)
        }

        public void OnPressAltBtn(Vector2 positionSS)
        {
        }

        public void OnDragAltBtn(Vector2 positionSS)
        {
            
        }

        public void OnReleaseAltBtn(Vector2 positionSS)
        {
        }

        public void OnUpKey()
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();

            focusTransform.position -= new Vector3(CrashDevOption.cameraMoveDelta * Time.deltaTime, 0, 0);
        }

        public void OnDownKey()
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();

            focusTransform.position += new Vector3(CrashDevOption.cameraMoveDelta * Time.deltaTime, 0, 0);
        }

        public void OnLeftKey()
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();

            focusTransform.position -= new Vector3(0, 0, CrashDevOption.cameraMoveDelta * Time.deltaTime);
        }

        public void OnRightKey()
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();
            
            focusTransform.position += new Vector3(0, 0, CrashDevOption.cameraMoveDelta * Time.deltaTime);
        }
    }
}