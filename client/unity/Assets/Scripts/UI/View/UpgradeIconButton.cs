using KG;
using Michsky.UI.ModernUIPack;
using Shared.Type;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class UpgradeEvent : UnityEvent<Ability> { }

    [RequireComponent(typeof(MultiClickButton))]
    public class UpgradeIconButton : UIAutoComponent<MultiClickButton, Icon, Progress, TextMeshProUGUI>
    {
        public enum State
        {
            Disabled,
            Ready,
            Pending,
            Progress,
            Finish
        }

        private State state { get; set; }

        [SerializeField] private UpgradeEvent _onStartClick;
        [SerializeField] private UpgradeEvent _onCancelClick;
        
        public UpgradeEvent onStartClick => _onStartClick;
        public UpgradeEvent onCancelClick => _onCancelClick;

        private Ability _ability;
        public Ability ability
        {
            get => _ability;
            set => _ability = value;
        }

        public Sprite icon
        {
            get => instance1.icon;
            set => instance1.icon = value;
        }
        
        public float progress
        {
            get => instance2.progress;
            set => instance2.progress = value;
        }

        public int? pendingOrder
        {
            set => instance3.text = value != null? value.Value.ToString(): "";
        }

        private void OnEnable()
        {
            instance0.maxButtonClickCount = 2;
            instance0.onClick.AddListener(OnUpgradeClick);
            
            SetState(State.Ready);
        }

        private void OnDisable()
        {
            instance0.onClick.RemoveListener(OnUpgradeClick);
            
            SetState(State.Disabled);
        }

        private void OnUpgradeClick(int count)
        {
            if (state == State.Disabled || state == State.Finish)
                return;
            
            if (count == 1)
            {
                onStartClick.Invoke(_ability);
            }
            else
            {
                SetState(State.Ready);
                onCancelClick.Invoke(_ability);
            }
        }

        public void SetState(State state)
        {
            this.state = state;
            
            switch (state)
            {
                case State.Disabled:
                case State.Ready:
                    instance3.gameObject.SetActive(false);
                    progress = 0;
                    pendingOrder = null;
                    instance3.text = "";
                    break;
                case State.Pending:
                    instance3.gameObject.SetActive(false);
                    progress = 0;
                    break;
                case State.Progress:
                    instance3.gameObject.SetActive(false);
                    instance3.text = "";
                    pendingOrder = null;
                    break;
                case State.Finish:
                    instance3.gameObject.SetActive(true);
                    instance3.text = "V";
                    progress = 1;
                    break;
            }
        }
    }
}