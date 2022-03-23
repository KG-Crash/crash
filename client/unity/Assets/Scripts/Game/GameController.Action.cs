using FixMath.NET;
using Shared;

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
        
        public void EnqueueSpeed(uint times)
        {
            EnqueueAction(new Protocol.Request.Action()
            {
                Frame = InputFrame,
                Id = (int) ActionKind.Speed,
                Param1 = ActionExtension.LOWORD(times),
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
        #endregion

        #region 수신
        public void OnActionPause(int frame, uint param1, uint param2)
        {
            var upgradeType = (Ability)param1.HIWORD();
        }
        #endregion
    }
}