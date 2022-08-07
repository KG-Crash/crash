using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using UnityEngine.SceneManagement;
using Protocol.Response;
using System.Linq;
using Lobby;
using UI;

public partial class LobbyState
{
    [FlatBufferEvent]
    public async Task<bool> OnCreateRoom(CreateRoom response)
    {
        await MoveStateAsync<GameRoomState>(new GameRoomTransition(
            true, new[] { Client.Instance.uuid })
        );
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnEnterRoom(EnterRoom response)
    {
        await MoveStateAsync<GameRoomState>(new GameRoomTransition(
            response.User == Client.Instance.uuid, 
            response.Users.Select(x => x.Id).ToArray()
        ));
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnRoomList(RoomList response)
    {
        GetView<LobbyPanel>().gameRoomList.Refresh(new RoomListListener(response));
        return true;
    }
}