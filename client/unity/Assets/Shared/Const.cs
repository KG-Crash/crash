// DO NOT MODIFY

namespace Shared.Const
{
    public static class Portal
    {
        public static readonly int Width = 1;
        public static readonly int Height = 2;
    }
    
    public static class Character
    {
        public static readonly string InitStat = "캐릭터.스탯";
        public static readonly double MoveEpsilon = 0.001;
    }
    
    public static class Input
    {
        public static readonly double CameraMoveDelta = 10;
        public static readonly double DragDelta = 0.1;
        public static readonly double ScrollDelta = 0.1;
    }
    
    public static class Time
    {
        public static readonly int FPS = 60;
        public static readonly int TPS = 8;
    }
}