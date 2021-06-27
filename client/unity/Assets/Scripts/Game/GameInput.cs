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
        private Vector2 _startPositionSS;
        
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
            _startPositionSS = positionSS;
            
            var diffSS = _startPositionSS - positionSS;
            diffSS = new Vector2(Mathf.Abs(diffSS.x), Mathf.Abs(diffSS.y));
            var mixSS = new Vector2(Mathf.Min(_startPositionSS.x, positionSS.x), Mathf.Min(_startPositionSS.y, positionSS.y));
            var dragRectSS = new Rect(mixSS, diffSS);

            _ui.ActiveDragRect(true);
            _ui.UpdateDragRect(dragRectSS);
        }

        public void OnDragMainBtn(Vector2 positionSS)
        {
            var diffSS = _startPositionSS - positionSS;
            diffSS = new Vector2(Mathf.Abs(diffSS.x), Mathf.Abs(diffSS.y));
            var mixSS = new Vector2(Mathf.Min(_startPositionSS.x, positionSS.x), Mathf.Min(_startPositionSS.y, positionSS.y));
            var dragRectSS = new Rect(mixSS, diffSS);
            //_controller.UpdateDragRect(dragRectSS);
            _ui.UpdateDragRect(dragRectSS);
        }
        public void OnReleaseMainBtn(Vector2 positionSS)
        {
            var diffSS = _startPositionSS - positionSS;
            diffSS = new Vector2(Mathf.Abs(diffSS.x), Mathf.Abs(diffSS.y));
            var mixSS = new Vector2(Mathf.Min(_startPositionSS.x, positionSS.x), Mathf.Min(_startPositionSS.y, positionSS.y));
            var dragRectSS = new Rect(mixSS, diffSS);
            
            _controller.UpdateDragRect(dragRectSS);           
            _ui.ActiveDragRect(false);
            _ui.UpdateDragRect(dragRectSS);
        }

        public void OnPressAltBtn(Vector2 positionSS)
        {
            
        }

        public void OnDragAltBtn(Vector2 positionSS)
        {
            
        }

        public void OnReleaseAltBtn(Vector2 positionSS)
        {
            _controller.MoveOrAttackTo(positionSS, out var isMove);

            if (isMove)
            {
                _ui.MoveTo(positionSS);
            }
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