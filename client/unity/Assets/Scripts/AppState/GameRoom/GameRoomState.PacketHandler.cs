using GameRoom;
using Network;
using Protocol.Response;
using System.Linq;
using System.Threading.Tasks;
using UI;
#pragma warning disable 1998

public partial class GameRoomState
{
    [FlatBufferEvent]
    public async Task<bool> OnLogin(Login response)
    {
        // 게임서버 연결됐을 때 처리
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnCreateRoom(CreateRoom response)
    {
        var view = GetView<GameRoomPanel>();
        view.userNameList.Refresh(new UserListListenerWithButton(uuid, new [] {uuid}));
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnEnterRoom(EnterRoom response)
    {
        var view = GetView<GameRoomPanel>();
        view.userNameList.Refresh(new UserListListenerWithButton(uuid, response.Users.Select(x => x.Id)));
        return true;
    }
    
    [FlatBufferEvent]
    public async Task<bool> OnGameStart(GameStart response)
    {
        if (response.Error != 0)
        {
            UnityEngine.Debug.LogError("게임을 시작할 수 없음");
            return true;
        }

        seed = response.Seed;
        _ = MoveStateAsync<GameState>();
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnLeaveRoom(LeaveRoom response)
    {
        var isMine = (response.User == uuid);

        if (isMine)
        {
            await MoveStateAsync<LobbyState>();
        }
        else
        {
            UnityEngine.Debug.Log($"{response.User} 가 나감.");
        }

        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnChat(Chat response)
    {
        UnityEngine.Debug.Log(response.Message);
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnWhisper(Whisper response)
    {
        if (uuid == response.From)
        {
            UnityEngine.Debug.Log($"{response.To} << {response.Message}");
        }
        else
        {
            UnityEngine.Debug.Log($"{response.From} >> {response.Message}");
        }
        return true;
    }
}