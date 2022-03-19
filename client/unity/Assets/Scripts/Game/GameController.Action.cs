using FixMath.NET;
using Shared;

namespace Game
{
    public partial class GameController
    {
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
    }
}