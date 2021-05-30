using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(GameController))]
    public class GameInput : MonoBehaviour, IInputSubscriber
    {
        private GameController _controller;
        private Vector2 _startPositionSS;
        
        private void Awake()
        {
            _controller = GetComponent<GameController>();
            
            InputBridge._instance.Register(this);
        }

        private void OnDestroy()
        {
            InputBridge._instance.Unregister(this);
        }

        public void OnPressMainBtn(Vector2 positionSS)
        {
            _startPositionSS = positionSS;
        }

        public void OnDragMainBtn(Vector2 positionSS)
        {
            var diffSS = _startPositionSS - positionSS;
            diffSS = new Vector2(Mathf.Abs(diffSS.x), Mathf.Abs(diffSS.y));
            var sumSS = _startPositionSS + positionSS;
            var dragRectSS = new Rect(sumSS / 2.0f, diffSS);
            //_controller.UpdateDragRect(dragRectSS);
        }
        public void OnReleaseMainBtn(Vector2 positionSS)
        {
            var diffSS = _startPositionSS - positionSS;
            diffSS = new Vector2(Mathf.Abs(diffSS.x), Mathf.Abs(diffSS.y));
            var sumSS = _startPositionSS + positionSS;
            var dragRectSS = new Rect(sumSS / 2.0f, diffSS);
            
            _controller.UpdateDragRect(dragRectSS);
        }

        public void OnUpKey()
        {
        }

        public void OnDownKey()
        {
        }

        public void OnLeftKey()
        {
        }

        public void OnRightKey()
        {
        }
    }
}