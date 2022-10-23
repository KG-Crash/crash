using FixMath.NET;
using Game;
using Shared.Type;
using UnityEngine;
using Action = Protocol.Response.Action;

public partial class GameState
{
    #region 송신

    public void EnqueueHeartBeat()
    {
        actionService.Send(new Protocol.Request.Action
        {
            Frame = LockStep.Frame.In,
            Id = (int) ActionKind.HeartBeat,
            Param1 = (uint) (TPS - LockStep.Frame.In),
            Param2 = (uint) LockStep.Frame.In
        });
    }

    public void EnqueueSpeed(int times)
    {
        actionService.Send(new Protocol.Request.Action
        {
            Frame = LockStep.Frame.In,
            Id = (int) ActionKind.Speed,
            Param1 = ActionExtension.TOWORD(0, (ushort) times),
            Param2 = 0
        });
    }

    public void EnqueuePause()
    {
        actionService.Send(new Protocol.Request.Action
        {
            Frame = LockStep.Frame.In,
            Id = (int)ActionKind.Pause
        });
    }

    public void EnqueueUpgradeFinish(Ability ability)
    {
        actionService.Send(new Protocol.Request.Action
        {
            Frame = LockStep.Frame.In,
            Id = (int) ActionKind.UpgradeFinish,
            Param1 = (uint)ability,
            Param2 = 0
        });
    }

    public void EnqueueSpawn(int type, uint count, FixVector2 pos)
    {
        actionService.Send(new Protocol.Request.Action
        {
            Frame = LockStep.Frame.In,
            Id = (int)ActionKind.Spawn,
            Param1 = ActionExtension.TOWORD((ushort) count, (ushort) type),
            Param2 = ActionExtension.TOWORD(pos)
        });
    }

    public void EnqueueAttackPlayer(uint playerID)
    {
        actionService.Send(new Protocol.Request.Action
        {
            Frame = LockStep.Frame.In,
            Id = (int)ActionKind.AttackPlayer,
            Param1 = ActionExtension.TOWORD(0, (ushort) playerID)
        });
    }

    public void EnqueueUpgradeCancel(Ability ability)
    {
        actionService.Send(new Protocol.Request.Action
        {
            Frame = LockStep.Frame.In,
            Id = (int) ActionKind.UpgradeCancel,
            Param1 = (uint)ability,
            Param2 = 0
        });
    }

    #endregion

    #region 수신

    [ActionHandler(ActionKind.HeartBeat)]
    public void OnActionHeartBeat(Action action, ActionHandleParam actionHandleParam)
    {
        // hearbeat 용
    }

    [ActionHandler(ActionKind.Pause)]
    public void OnActionPause(Action action, ActionHandleParam actionHandleParam)
    {
        paused = true;
    }

    [ActionHandler(ActionKind.Speed)]
    public void OnActionSpeed(Action action, ActionHandleParam actionHandleParam)
    {
        Debug.Log($"현재 {TimeSpeed} 배속");
        TimeSpeed = Fix64.One * action.Param1.LOWORD();
    }

    [ActionHandler(ActionKind.UpgradeFinish)]
    public void OnActionUpgradeFinish(Action action, ActionHandleParam actionHandleParam)
    {
        var ability = (Ability) action.Param1;
        var player = teams.Find(actionHandleParam.userId);
        player.upgrade.AddAbility(ability);
        
        if (player == me)
            OnUpgradeFinishUI(ability);
    }
    
    [ActionHandler(ActionKind.UpgradeCancel)]
    public void OnActionUpgradeCancel(Action action, ActionHandleParam actionHandleParam)
    {
        var ability = (Ability) action.Param1;
        var player = teams.Find(actionHandleParam.userId);
        player.upgrade.RemoveAbility(ability);
        
        if (player == me)
            OnUpgradeCancelUI(ability);
    }

    [ActionHandler(ActionKind.AttackPlayer)]
    public void OnActionAttackPlayer(Action action, ActionHandleParam actionHandleParam)
    {
        var targetPlayerNumber = action.Param1.LOWORD();
        var attacker = teams.Find(actionHandleParam.userId);
        attacker.target = teams.Find(targetPlayerNumber);
    }

    [ActionHandler(ActionKind.Spawn)]
    public void OnActionSpawn(Action action, ActionHandleParam actionHandleParam)
    {
        var unitType = action.Param1.LOWORD();
        var count = action.Param1.HIWORD();
        var pos = action.Param2.WORD2POS();
        var playerId = actionHandleParam.userId;
        var player = teams.Find(playerId);

        for (var i = 0; i < count; i++)
            player.units.Add(unitType, map, pos);
    }

    #endregion
}