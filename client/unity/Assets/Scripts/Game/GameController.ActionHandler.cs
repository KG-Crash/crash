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
            EnqueueAction(new Protocol.Request.Action
            {
                Frame = InputFrame,
                Id = (int)ActionKind.HeartBeat,
                Param1 = (uint)(TPS - InputFrame),
                Param2 = (uint)InputFrame
            });
        }
        
        public void EnqueueSpeed(int times)
        {
            EnqueueAction(new Protocol.Request.Action()
            {
                Frame = InputFrame,
                Id = (int) ActionKind.Speed,
                Param1 = ActionExtension.LOWORD((uint)times),
                Param2 = 0
            });
        }
        
        public void EnqueuePause()
        {
            EnqueueAction(new Protocol.Request.Action
            {
                Frame = InputFrame,
                Id = (int)Shared.ActionKind.Pause
            });
        }

        public void EnqueueUpgrade(Ability ability)
        {
            EnqueueAction(new Protocol.Request.Action
            { 
                Frame = InputFrame,
                Id = (int)Shared.ActionKind.Pause,
                Param1 = ActionExtension.HIWORD((uint)ability),
                Param2 = 0
            });
        }

        public void EnqueueSpawn(uint unitType, uint count, FixVector2 pos)
        {
            EnqueueAction(new Protocol.Request.Action
            {
                Frame = InputFrame,
                Id = (int)Shared.ActionKind.Spawn,
                Param1 = ActionExtension.LOWORD(unitType),
                Param2 = (uint)ActionExtension.HIWORD((uint)pos.x) | ActionExtension.HIWORD((uint)pos.y)
            });
        }

        public void EnqueueAttack(uint playerID)
        {
            EnqueueAction(new Protocol.Request.Action
            {
                Frame = InputFrame,
                Id = (int)Shared.ActionKind.AttackPlayer,
                Param1 = ActionExtension.HIWORD(playerID)
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
            //Debug.Log("on spawn unit ");
            //var unitType = action.Param1.LOWORD();
            //var x = action.Param2.HIWORD();
            //var y = action.Param2.LOWORD();
            //var playerId = actionHandleParam.userId;

            //Fix64 _startRadian = Fix64.Zero;
            //FixVector2 rot = GetSpawnRotation(playerId);            

            //var ctx = new TemporalPlaceContext();
            //var position = new FixVector2((int)x, (int)y); 
            //Player player = GetPlayer((uint)playerId);

            ////SpawnUnitToPosition(unitType, player, position, ctx);
            //SpawnUnitToPlayerStart(unitType, player, ctx);
        }
        #endregion
        
    }
}
