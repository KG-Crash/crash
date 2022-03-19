// DO NOT MODIFY

namespace Shared
{
    public enum ClientExceptionCode
    {
        InvalidCellAccess,
        NotWalkableNextCell,
        ZeroCellPath,
        NotFoundUIAttribute,
        NotContainUIScript
    }

    public enum ResultCode
    {
        None,
        InvalidUser,
        NoPrivilege,
        AlreadyPlaying,
        NotEnoughUsers,
        NotEnoughTeams,
        NotFoundGameRoom,
        AlreadyEnteredGameRoom,
        NotEnteredAnyGameRoom,
        FullUsers,
        NotPlayingState
    }

    public enum ActionKind
    {
        HeartBeat,
        Pause,
        Speed,
        Upgrade,
        AttackPlayer,
        Spawn
    }

    public enum ProjectileState
    {
        Disable,
        Shoot,
        Move,
        Hit
    }

    public enum ProjectileType
    {
        Absolute,
        Relative
    }

    public enum UnitSize
    {
        Small,
        Medium,
        Large
    }

    public enum UnitState
    {
        Idle,
        Move,
        Attack,
        Dead
    }

    public enum UnitType
    {
        Normal,
        Explosive,
        Concussive
    }

    public enum StatType
    {
        Hp,
        Damage,
        Armor,
        AttackRange,
        Speed,
        AttackSpeed
    }

    public enum CommandType
    {
        Move,
        AttackSingleTarget,
        AttackMultiTarget
    }

    public enum AttackType
    {
        Immediately,
        Projectile
    }

    public enum Ability
    {
        NONE = 0x00000000,
        UPGRADE_1 = 0x00000001,
        UPGRADE_2 = 0x00000002,
        UPGRADE_3 = 0x00000004,
        UPGRADE_4 = 0x00000008,
        UPGRADE_5 = 0x00000010,
        UPGRADE_6 = 0x00000020,
        UPGRADE_7 = 0x00000040,
        UPGRADE_8 = 0x00000080,
        UPGRADE_9 = 0x00000100,
        UPGRADE_10 = 0x00000200,
        UPGRADE_11 = 0x00000400,
        UPGRADE_12 = 0x00000800,
        UPGRADE_13 = 0x00001000,
        UPGRADE_14 = 0x00002000,
        UPGRADE_15 = 0x00004000,
        UPGRADE_16 = 0x00008000,
        UPGRADE_17 = 0x00010000,
        UPGRADE_18 = 0x00020000,
        UPGRADE_19 = 0x00040000,
        UPGRADE_20 = 0x00080000
    }

    public enum Advanced
    {
        UPGRADE_WEAPON,
        UPGRADE_ARMOR,
        UPGRADE_SPEED
    }
}