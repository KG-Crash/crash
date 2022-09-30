using FixMath.NET;
using Game;

public partial class GameState
{
    public static int FPS { get; set; }
    public static int TPS { get; set; }
    public static Fix64 TimeSpeed { get; set; } = Fix64.One;
    public static Fix64 TimeDelta => (Fix64.One * TimeSpeed) / new Fix64(FPS);
    public static Fix64 TurnRate => Fix64.One / new Fix64(8);
    public static bool paused { get; set; }
    public static bool ready { get; set; }
    public static bool waitPacket { get; set; }

    public static int InputTotalFrame => LockStep.Frame.In + LockStep.Turn.In * Shared.Const.Time.FramePerTurn;

    public static Frame InputFrameChunk => new Frame()
        {currentFrame = LockStep.Frame.In, currentTurn = LockStep.Turn.In, deltaTime = TimeDelta};

    public static int OutputTotalFrame => LockStep.Frame.Out + LockStep.Turn.Out * Shared.Const.Time.FramePerTurn;

    public static Frame OutputFrameChunk => new Frame()
        {currentFrame = LockStep.Frame.Out, currentTurn = LockStep.Turn.Out, deltaTime = TimeDelta};
}