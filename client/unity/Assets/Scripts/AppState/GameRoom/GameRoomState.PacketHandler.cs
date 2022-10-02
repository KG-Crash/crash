using System;
using System.Linq;
using System.Threading.Tasks;
using GameRoom;
using Module;
using Network;
using Protocol.Response;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameRoomState
{
    [FlatBufferEvent]
    public async Task<bool> OnGameStart(GameStart response)
    {
        if (response.Error != 0)
        {
            UnityEngine.Debug.LogError("게임을 시작할 수 없음");
            return true;
        }

        Client.Instance.seed = response.Seed;
        _ = MoveStateAsync<GameState>();
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnLeaveRoom(LeaveRoom response)
    {
        var isMine = (response.User == Client.Instance.uuid);

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
        if (Client.Instance.uuid == response.From)
        {
            UnityEngine.Debug.Log($"{response.To} << {response.Message}");
        }
        else
        {
            UnityEngine.Debug.Log($"{response.From} >> {response.Message}");
        }
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnEnterRoom(EnterRoom response)
    {
        var view = GetView<GameRoomPanel>();
        view.userNameList.Refresh(new UserListListenerWithButton(Client.Instance.uuid, response.Users.Select(x => x.Id)));
        return true;
    }
}