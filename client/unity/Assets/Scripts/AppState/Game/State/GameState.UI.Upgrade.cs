using Game;
using Shared.Type;
using UI;

public partial class GameState
{
    private void InitializeUpgradePanel()
    {
        var upgradePanel = GetView<UpgradePanel>();
        upgradePanel.Initialize();
        upgradePanel.exitClick.AddListener(OnUpgradeExit);
        upgradePanel.upgradeClickEvent.AddListener(OnUpgradeClick);
        upgradePanel.upgradeCancelEvent.AddListener(OnUpgradeCancelClick);
    }

    [UpdateLockStep]
    private void UpdateUpgradePanel(Frame input, Frame output)
    {
        GetView<UpgradePanel>().OnSimulateProgress(output, me.upgrade);
    }

    private void ClearUpgradePanel()
    {
        var upgradePanel = GetView<UpgradePanel>();
        upgradePanel.exitClick.RemoveListener(OnUpgradeExit);
        upgradePanel.upgradeClickEvent.RemoveListener(OnUpgradeClick);
        upgradePanel.upgradeCancelEvent.RemoveListener(OnUpgradeCancelClick);
    }

    private void OnUpgradeExit()
    {
        _ = CloseTopView<UpgradePanel>();
    }

    private void OnUpgradeClick(Ability ability)
    {
        if ((me.upgrade.abilities & ability) > 0)
            return;
        
        if (me.upgrade.inUpgradeProgress)
        {
            me.upgrade.Reserve(ability);
        }
        else
        {
            var totalFrame = (uint) (LockStep.Frame.Out + LockStep.Turn.Out * Shared.Const.Time.FramePerTurn);
            me.upgrade.Start(ability, totalFrame);
        }
    }

    private void OnUpgradeCancelClick(Ability ability)
    {
        if ((me.upgrade.abilities & ability) > 0)
            return;

        var totalFrame = (uint) (LockStep.Frame.Out + LockStep.Turn.Out * Shared.Const.Time.FramePerTurn);
        me.upgrade.CancelAbility(ability, totalFrame);
    }

    private void OnUpgradeFinishUI(Ability ability)
    {
        var upgradePanel = GetView<UpgradePanel>();
        upgradePanel.OnUpgradeFinish(ability);
    }

    private void OnUpgradeCancelUI(Ability ability)
    {
        var upgradePanel = GetView<UpgradePanel>();
        upgradePanel.OnUpgradeCancel(ability);
    }
}