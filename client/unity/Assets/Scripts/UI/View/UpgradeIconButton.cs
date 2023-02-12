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
        private UpgradeStatus status { get; set; }

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

        protected override void OnEnable()
        {
            instance0.maxButtonClickCount = 2;
            instance0.onClick.AddListener(OnUpgradeClick);
        }

        protected override void OnDisable()
        {
            instance0.onClick.RemoveListener(OnUpgradeClick);
        }

        private void OnUpgradeClick(int count)
        {
            if (status == UpgradeStatus.Finish)
                return;
            
            if (count == 1)
            {
                onStartClick.Invoke(_ability);
            }
            else
            {
                SetState(UpgradeStatus.Ready);
                onCancelClick.Invoke(_ability);
            }
        }

        public void SetState(UpgradeStatus status)
        {
            this.status = status;
            
            switch (status)
            {
                case UpgradeStatus.Ready:
                    instance3.gameObject.SetActive(false);
                    progress = 0;
                    pendingOrder = null;
                    instance3.text = "";
                    break;
                case UpgradeStatus.Pending:
                    instance3.gameObject.SetActive(false);
                    progress = 0;
                    break;
                case UpgradeStatus.Progress:
                    instance3.gameObject.SetActive(false);
                    instance3.text = "";
                    pendingOrder = null;
                    break;
                case UpgradeStatus.Finish:
                    instance3.gameObject.SetActive(true);
                    instance3.text = "V";
                    progress = 1;
                    break;
            }
        }
    }
}