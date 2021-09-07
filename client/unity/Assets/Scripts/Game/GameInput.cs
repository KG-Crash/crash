using System;
using System.Collections.Generic;
using FixMath.NET;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(GameController), typeof(GameUI))]
    public class GameInput : MonoBehaviour, IInputSubscriber
    {
        private GameController _controller;
        private GameUI _ui;
        private Vector2 _lastPositionSS;
        
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
            _lastPositionSS = positionSS;
        }

        public void OnDragMainBtn(Vector2 positionSS)
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();
            var dragDelta = (float)Shared.Const.Input.DragDelta;
            
            var deltaSS = (positionSS - _lastPositionSS) * dragDelta;
            focusTransform.position += new Vector3(deltaSS.y, 0, -deltaSS.x);

            _lastPositionSS = positionSS;
        }
        public void OnReleaseMainBtn(Vector2 positionSS)
        {            
            _lastPositionSS = positionSS;
        }

        public void OnPressAltBtn(Vector2 positionSS)
        {
        }

        public void OnDragAltBtn(Vector2 positionSS)
        {
            
        }

        public void OnReleaseAltBtn(Vector2 positionSS)
        {
            var positionWS = _controller.ScreenPositionToWorldPosition(positionSS);

            _controller.SpawnUnitToPosition(_controller._spawnUnitOriginID, _controller._spawnPlayerID, positionWS);
        }

        public void OnUpKey()
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();
            var cameraMoveDelta = (float)Shared.Const.Input.CameraMoveDelta;

            focusTransform.position -= new Vector3(cameraMoveDelta * Time.deltaTime, 0, 0);
        }

        public void OnDownKey()
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();
            var cameraMoveDelta = (float)Shared.Const.Input.CameraMoveDelta;

            focusTransform.position += new Vector3(cameraMoveDelta * Time.deltaTime, 0, 0);
        }

        public void OnLeftKey()
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();
            var cameraMoveDelta = (float)Shared.Const.Input.CameraMoveDelta;

            focusTransform.position -= new Vector3(0, 0, cameraMoveDelta * Time.deltaTime);
        }

        public void OnRightKey()
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();
            var cameraMoveDelta = (float)Shared.Const.Input.CameraMoveDelta;
            
            focusTransform.position += new Vector3(0, 0, cameraMoveDelta * Time.deltaTime);
        }

        public void OnScrollDelta(float onScrollDelta)
        {
            var unityObject = UnityResources._instance.Get("objects");
            var follower = unityObject.GetCameraFollower();

            follower.offsetPosition += onScrollDelta *  (float)Shared.Const.Input.ScrollDelta;
        }

        public void OnScroll(float delta)
        {
            
        }
    }
}