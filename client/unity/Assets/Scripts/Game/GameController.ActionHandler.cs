using FixMath.NET;
using Shared;
using UnityEngine;
using Action = Protocol.Response.Action;

namespace Game
{
    public partial class GameController
    {
        #region 송신
        public void EnqueueHeartBeat()
        {
            ActionService.Send(new Protocol.Request.Action
            {
                Frame = LockStep.Frame.In,
                Id = (int)ActionKind.HeartBeat,
                Param1 = (uint)(TPS - LockStep.Frame.In),
                Param2 = (uint)LockStep.Frame.In
            });
        }
        
        public void EnqueueSpeed(int times)
        {
            ActionService.Send(new Protocol.Request.Action
            {
                Frame = LockStep.Frame.In,
                Id = (int)ActionKind.Speed,
                Param1 = ActionExtension.TOWORD(0, (ushort)times),
                Param2 = 0
            });
        }
        
        public void EnqueuePause()
        {
            ActionService.Send(new Protocol.Request.Action
            {
                Frame = LockStep.Frame.In,
                Id = (int)Shared.ActionKind.Pause
            });
        }

        public void EnqueueUpgrade(Ability ability)
        {
            ActionService.Send(new Protocol.Request.Action
            { 
                Frame = LockStep.Frame.In,
                Id = (int)Shared.ActionKind.Pause,
                Param1 = ActionExtension.TOWORD(0, (ushort)ability),
                Param2 = 0
            });
        }

        public void EnqueueSpawn(int type, uint count, FixVector2 pos)
        {
            ActionService.Send(new Protocol.Request.Action
            {
                Frame = LockStep.Frame.In,
                Id = (int)Shared.ActionKind.Spawn,
                Param1 = ActionExtension.TOWORD((ushort)count, (ushort)type),
                Param2 = ActionExtension.TOWORD(pos)
            });
        }

        public void EnqueueAttack(uint playerID)
        {
            ActionService.Send(new Protocol.Request.Action
            {
                Frame = LockStep.Frame.In,
                Id = (int)Shared.ActionKind.AttackPlayer,
                Param1 = ActionExtension.TOWORD(0, (ushort)playerID)
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
        
        [ActionHandler(ActionKind.Upgrade)]
        public void OnActionUpgrade(Action action, ActionHandleParam actionHandleParam)
        {
        }
        
        [ActionHandler(ActionKind.AttackPlayer)]
        public void OnActionAttackPlayer(Action action, ActionHandleParam actionHandleParam)
        {
            var targetPlayerNumber = action.Param1.LOWORD();
            if (actionHandleParam.userId == targetPlayerNumber)
                return;

            var attacker = _teams.Find(actionHandleParam.userId);
            attacker.target = _teams.Find(targetPlayerNumber);
        }

        [ActionHandler(ActionKind.Spawn)]
        public void OnActionSpawn(Action action, ActionHandleParam actionHandleParam)
        {
            Debug.Log("on spawn unit ");
            var unitType = action.Param1.LOWORD();
            var count = action.Param1.HIWORD();
            var pos = action.Param2.WORD2POS();
            var playerId = actionHandleParam.userId;
            var player = _teams.Find(playerId);
            
            for (var i = 0; i < count; i++)
                player.units.Add(unitType, map, pos);
        }
        #endregion
        
    }
}
