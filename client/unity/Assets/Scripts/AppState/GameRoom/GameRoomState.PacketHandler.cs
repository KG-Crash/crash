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
        // TODO : Request-Response 인 경우 핸들러가 있어야 넘어감 ㅠ 
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnCreateRoom(CreateRoom response)
    {
        var view = GetView<GameRoomPanel>();
        view.userNameList.Refresh(new UserListListenerWithButton(CrashNetwork.uuid, new [] {CrashNetwork.uuid}));
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnEnterRoom(EnterRoom response)
    {
        var view = GetView<GameRoomPanel>();
        view.userNameList.Refresh(new UserListListenerWithButton(CrashNetwork.uuid, response.Users.Select(x => x.Id)));
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

        CrashNetwork.seed = response.Seed;
        _ = MoveStateAsync<GameState>();
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnLeaveRoom(LeaveRoom response)
    {
        var isMine = (response.User == CrashNetwork.uuid);

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
        var view = GetView<GameRoomPanel>();
        view.chattingView.chatLog.Refresh(new ChatLogListener(response));
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnWhisper(Whisper response)
    {
        if (CrashNetwork.uuid == response.From)
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