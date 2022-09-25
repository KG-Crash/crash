using Game;
using Shared.Type;
using UI;

public partial class GameState
{
    public void InitializeUpgradePanel()
    {
        var upgradePanel = GetView<UpgradePanel>();
        upgradePanel.Initialize();
        upgradePanel.exitClick.AddListener(OnUpgradeExit);
        upgradePanel.upgradeClick.AddListener(OnUpgradeClick);
    }

    public void ClearUpgradePanel()
    {
        var upgradePanel = GetView<UpgradePanel>();
        upgradePanel.exitClick.RemoveListener(OnUpgradeExit);
        upgradePanel.upgradeClick.RemoveListener(OnUpgradeClick);
    }

    private void OnUpgradeExit()
    {
        _ = CloseTopView<UpgradePanel>();
    }

    private void OnUpgradeClick(Ability ability)
    {
        _me.upgrade.Start(ability, LockStep.Frame.In);
    }
}