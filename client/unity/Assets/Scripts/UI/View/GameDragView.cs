using System;
using System.Collections;
using System.Collections.Generic;
using KG;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    [Serializable]
    public class GameDragEvent : UnityEvent<Vector2> { }

    [RequireComponent(typeof(GraphicRaycastTarget))]
    public class GameDragView : UIComponent, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private GameDragEvent _gameDragEvent;
        private Vector2 lastPosition;

        public GameDragEvent gameDragEvent => _gameDragEvent;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            lastPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var delta = eventData.position - lastPosition;
            _gameDragEvent.Invoke(delta);

            lastPosition = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            lastPosition = eventData.position;
        }
    }
}