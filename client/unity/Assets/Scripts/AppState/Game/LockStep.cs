namespace Game
{
    public static class LockStep
    {
        public class Pair
        {
            public int In { get; set; }
            public int Out { get; set; }
        }

        public static Pair Frame { get; private set; } = new Pair();
        public static Pair Turn { get; private set; } = new Pair();

        public static void Reset()
        {
            Frame.In = Frame.Out = Turn.In = Turn.Out = 0;
        }
    }
}