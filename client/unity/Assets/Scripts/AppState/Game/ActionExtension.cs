using FixMath.NET;

namespace Game
{
    public static class ActionExtension
    {
        public static ushort LOWORD(this uint actionParam) => (ushort) (actionParam & 0x0000FFFF);
        public static ushort HIWORD(this uint actionParam) => (ushort) ((actionParam & 0xFFFF0000) >> 16);

        public static FixVector2 WORD2POS(this uint actionParam) =>
            new FixVector2(Fix64.One * actionParam.LOWORD(), Fix64.One * actionParam.HIWORD());

        public static uint TOWORD(ushort hiword, ushort loword)
        {
            return loword | (uint) hiword << 16;
        }

        public static uint TOWORD(FixVector2 v2)
        {
            return (uint) v2.x | (uint) v2.y << 16;
        }

        public static uint SetParam1LOWORD(this Protocol.Request.Action action, ushort value)
        {
            action.Param1 = (action.Param1 & 0xFFFF0000) + (uint) (value & 0x0000FFFF);
            return action.Param1;
        }

        public static uint SetParam1HIWORD(this Protocol.Request.Action action, ushort value)
        {
            action.Param1 = (action.Param1 & 0x0000FFFF) + (uint) ((value & 0x0000FFFF) << 16);
            return action.Param1;
        }

        public static uint SetParam2LOWORD(this Protocol.Request.Action action, ushort value)
        {
            action.Param2 = (action.Param2 & 0xFFFF0000) + (uint) (value & 0x0000FFFF);
            return action.Param2;
        }

        public static uint SetParam2HIWORD(this Protocol.Request.Action action, ushort value)
        {
            action.Param2 = (action.Param2 & 0x0000FFFF) + (uint) ((value & 0x0000FFFF) << 16);
            return action.Param2;
        }
    }
}