using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using Network;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Protocol;
using Protocol.Request;
using UI;

[UIBind(typeof(LobbyPanel), true)]
[StateBind(flatBuffer: true)]
public partial class LobbyState : AppState
{
    private DateTime _lastUpdatedRefreshDate = DateTime.Now;
    public const int REFRESH_ROOM_INTERVAL = 10;
    
    public LobbyState() : base() {}
    
    [InitializeMethod]
    public void Initialize()
    {
        var view = GetView<LobbyPanel>();
        view.createGameRoomButtonClick.AddListener(OnCreateGameRoom);
        view.refreshButton.AddListener(OnRefreshRoom);
        
        _ = Refresh();
        
        //StartCoroutine(RefreshRoom(REFRESH_ROOM_INTERVAL));
    }

    [ClearMethod]
    public void Clear()
    {
        var view = GetView<LobbyPanel>();
        view.createGameRoomButtonClick.RemoveListener(OnCreateGameRoom);
        view.refreshButton.RemoveListener(OnRefreshRoom);
    }
    
    public async Task Refresh()
    {
        var response = await Request<Protocol.Response.RoomList>("lobby/room", new Protocol.Request.RoomList
        { });

        // TODO: 기능복구
        UnityEngine.Debug.Log(response);
    }

    public async Task ConnectAsCreate()
    {
        var response = await Request<Protocol.Response.RouteCreate>(
            "lobby/create-room", new Protocol.Request.RouteCreate()
        );
        await MoveStateAsync<GameRoomState>(new GameRoomTransition() {
            Host = response.Host, Port = response.Port, Enter = false, RouteCreate = response
        });
    }

    public async Task ConnectAsEnter(string RoomId)
    {
        var response = await Request<Protocol.Response.RouteEnter>(
            "lobby/enter-room", new Protocol.Request.RouteEnter() { Id = RoomId }
        );
        await MoveStateAsync<GameRoomState>(new GameRoomTransition() {
            Host = response.Host, Port = response.Port, Enter = true, RouteEnter = response, RoomId = RoomId
        });
    }

    public async void OnCreateGameRoom()
    {
        await ConnectAsCreate();
    }

    private void OnRefreshRoom()
    {
        Refresh().AsUniTask().Forget();
    }

    public IEnumerator RefreshRoom(int seconds) 
    {
        while (true)
        {
            var elapsedTime = DateTime.Now - _lastUpdatedRefreshDate;
            if (elapsedTime.TotalSeconds < seconds)
            {
                yield return new WaitForSeconds(seconds - (int)elapsedTime.TotalSeconds);
            }
            else
            {
                yield return new WaitForSeconds(seconds);
            }

            _lastUpdatedRefreshDate = DateTime.Now;
            _ = Refresh();
            UnityEngine.Debug.Log("request room list");
        }
    }
}