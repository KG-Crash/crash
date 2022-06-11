// DO NOT MODIFY DIRECTLY

using System;

namespace Shared.Type
{
    public enum ResultCode
    {
        None, // 유효하지 않은 유저
        InvalidUser, // 권한이 없음
        NoPrivilege, // 이미 게임 진행중
        AlreadyPlaying, // 유저 수 부족
        NotEnoughUsers, // 팀 수 부족
        NotEnoughTeams, // 게임 룸을 찾을 수 없음
        NotFoundGameRoom, // 이미 게임룸 참여중
        AlreadyEnteredGameRoom, // 게임 룸에 참여중이 아님
        NotEnteredAnyGameRoom, // 유저 다 찼음
        FullUsers, // 게임중이 아님
        NotPlayingState
    }
}