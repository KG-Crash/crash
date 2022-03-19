using FixMath.NET;
using Shared;

namespace Game
{
    public partial class GameController
    {
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
    }
}