using Game;
using KG;
using Shared.Type;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UpgradePanel  : KG.UIView
    {
        [SerializeField] private UpgradeIconButton _upgradeIconButtonPrefab;
        [SerializeField] private ButtonSingleTMP _exitButton;
        [SerializeField] private Transform _iconParent;
        [SerializeField] private UpgradeClickEvent _upgradeClick;
        
        private List<UpgradeIconButton> _upgradeIconButtons = new List<UpgradeIconButton>();
        
        public UpgradeClickEvent upgradeClick => _upgradeClick;
        public Button.ButtonClickedEvent exitClick => _exitButton.onClick;

        public void Initialize()
        {
            var table = UpgradeIconTable.Get();
            foreach (var ability in (Ability[])Enum.GetValues(typeof(Ability)))
            {
                if (ability == Ability.NONE) continue;
                
                var instance = Instantiate(_upgradeIconButtonPrefab, _iconParent);
                instance.ability = ability;
                instance.icon = table[ability];
                _upgradeIconButtons.Add(instance);
            }
        }

        private void OnEnable()
        {
            _upgradeIconButtons.ForEach(x => x.onClick.AddListener(OnUpgradeClick));
        }
        
        private void OnDisable()
        {
            _upgradeIconButtons.ForEach(x => x.onClick.RemoveListener(OnUpgradeClick));
        }

        private void OnUpgradeClick(Ability ability)
        {
            _upgradeClick.Invoke(ability);
        }
    }
}