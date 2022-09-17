using System;
using System.Collections;
using System.Collections.Generic;
using KG;
using Shared.Table;
using UnityEngine;
using System.Linq;
using Game;
using Shared;
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
        private UpgradeIconTable _table;
        
        public UpgradeClickEvent upgradeClick => _upgradeClick;
        public Button.ButtonClickedEvent exitClick => _exitButton.onClick;

        public void Initialize(UpgradeIconTable table)
        {
            _table = table;
            foreach (var ability in (Ability[])Enum.GetValues(typeof(Ability)))
            {
                if (ability == Ability.NONE) continue;
                
                var instance = Instantiate(_upgradeIconButtonPrefab, _iconParent);
                instance.ability = ability;
                instance.icon = _table[ability];
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