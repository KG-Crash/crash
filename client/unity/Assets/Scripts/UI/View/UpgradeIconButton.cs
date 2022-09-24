using KG;
using Michsky.UI.ModernUIPack;
using Shared.Type;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class UpgradeClickEvent : UnityEvent<Ability> { }

    public class UpgradeIconButton : UIAutoComponent<Button, ButtonManagerBasicIcon>
    {
        [SerializeField]
        private UpgradeClickEvent onUpgradeClick;
        public new UpgradeClickEvent onClick => onUpgradeClick;

        private Ability _ability;
        public Ability ability
        {
            get => _ability;
            set => _ability = value;
        }

        public Sprite icon
        {
            get => instance1.buttonIcon;
            set => instance1.buttonIcon = value;
        }

        private void OnEnable()
        {
            instance0.onClick.AddListener(OnUpgradeClick);
        }

        private void OnDisable()
        {
            instance0.onClick.RemoveListener(OnUpgradeClick);
        }

        private void OnUpgradeClick()
        {
            onUpgradeClick.Invoke(_ability);
        }
    }
}