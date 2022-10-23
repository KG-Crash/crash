using Game;
using KG;
using Shared.Type;
using System;
using System.Collections.Generic;
using Shared.Table;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UpgradePanel  : KG.UIView
    {
        [SerializeField] private UpgradeIconButton _upgradeIconButtonPrefab;
        [SerializeField] private ButtonSingleTMP _exitButton;
        [SerializeField] private Transform _iconParent;
        
        [SerializeField] private UpgradeEvent _upgradeClickEvent;
        [SerializeField] private UpgradeEvent _upgradeCancelEvent;

        private List<UpgradeIconButton> _upgradeIconButtons = new List<UpgradeIconButton>();
        
        public UpgradeEvent upgradeClickEvent => _upgradeClickEvent;
        public UpgradeEvent upgradeCancelEvent => _upgradeCancelEvent;
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
                instance.SetState(UpgradeStatus.Ready);
                instance.onStartClick.AddListener(OnUpgradeClick);
                instance.onCancelClick.AddListener(OnUpgradeLongClick);
                
                _upgradeIconButtons.Add(instance);
            }
        }
        
        private void OnUpgradeClick(Ability ability)
        {
            _upgradeClickEvent.Invoke(ability);
        }
        
        private void OnUpgradeLongClick(Ability ability)
        {
            _upgradeCancelEvent.Invoke(ability);
        }

        public void OnSimulateProgress(Frame output, Upgrade upgrade)
        {
            var outputTotalFrame = output.currentFrame + output.currentTurn * Shared.Const.Time.FramePerTurn;
            var upgradeIcon = _upgradeIconButtons.Find(x => x.ability == upgrade.currentUpdateAbility);
            var table = Table.From<TableUnitUpgradeCost>();

            if (upgrade.currentUpdateAbility != null && upgrade.updateStartFrame != null)
            {
                upgradeIcon.SetState(UpgradeStatus.Progress);
                var upgradeStartFrame = upgrade.updateStartFrame.Value;
                var upgradeFrame = table[upgrade.currentUpdateAbility.Value].DurationSec * Shared.Const.Time.FPS;
                upgradeIcon.progress = Mathf.Min((float)(outputTotalFrame - upgradeStartFrame) / upgradeFrame, 1);
            }

            var abilityList = upgrade.reservedAbilities;
            for (var i = 0; i < abilityList.Count; i++)
            {
                var willUpgradeIcon = _upgradeIconButtons.Find(x => x.ability == abilityList[i]);
                willUpgradeIcon.SetState(UpgradeStatus.Pending);
                willUpgradeIcon.pendingOrder = i + 1;
            }
        }

        public void OnUpgradeFinish(Ability ability)
        {
            var icon = _upgradeIconButtons.Find(x => x.ability == ability);
            icon.SetState(UpgradeStatus.Finish);
        }
        
        public void OnUpgradeCancel(Ability ability)
        {
            var icon = _upgradeIconButtons.Find(x => x.ability == ability);
            icon.SetState(UpgradeStatus.Ready);
        }
    }
}