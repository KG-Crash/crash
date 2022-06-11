// DO NOT MODIFY DIRECTLY

using System;

namespace Shared.Type
{
    public enum ActionKind
    {
        HeartBeat, // 일시정지 세팅
        Pause, // 배속
        Speed, // 업그레이드
        Upgrade, // 플레이어 위치로 공격
        AttackPlayer, // 유닛 스폰
        Spawn
    }
}