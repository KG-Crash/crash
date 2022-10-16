using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KG
{
    public class MultiClickButton : Selectable
    {            
        [Serializable]
        public class ButtonClickedEvent : UnityEvent<int> { }

        [SerializeField]
        private float _buttonUpThreshold = 0.25f;
        [SerializeField] 
        private uint _maxButtonClickCount = 2;
        [SerializeField]
        private ButtonClickedEvent _onClick;

        private SelectionState prevSelectionState { get; set; }
        private Coroutine clickReserved { get; set; }
        private int clickCount { get; set; }

        public float buttonUpThreshold { get => _buttonUpThreshold; set => _buttonUpThreshold = value; }
        public uint maxButtonClickCount { get => _maxButtonClickCount; set => _maxButtonClickCount = value; }
        public ButtonClickedEvent onClick => _onClick;

        protected override void Awake()
        {
            base.Awake();
            prevSelectionState = currentSelectionState;
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            Debug.Log($"{prevSelectionState} => {state}");
            
            Fire(state);
            prevSelectionState = state;
        }

        private void Fire(SelectionState state)
        {
            if (prevSelectionState == SelectionState.Pressed && state == SelectionState.Selected)
            {
                if (_maxButtonClickCount <= clickCount)
                    OnClick();
                else
                    clickReserved = StartCoroutine(InvokeAfter(_buttonUpThreshold, OnClick));
            }

            if (state == SelectionState.Pressed && prevSelectionState != state)
            {
                clickCount++;
                if (clickReserved != null)
                {
                    StopCoroutine(clickReserved);
                    clickReserved = null;   
                }
            }
        }

        private IEnumerator InvokeAfter(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action.Invoke();
        }

        private void OnClick()
        {
            _onClick.Invoke(clickCount);
            clickReserved = null;
            clickCount = 0;
        }
    }
}
