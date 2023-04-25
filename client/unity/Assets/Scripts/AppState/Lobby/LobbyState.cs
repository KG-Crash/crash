using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using Network;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Protocol.Request;
using UI;

[UIBind(typeof(LobbyPanel), true)]
[StateBind(flatBuffer: true)]
public partial class LobbyState : AppState
{
    private DateTime _lastUpdatedRefreshDate = DateTime.Now;
    public const int REFRESH_ROOM_INTERVAL = 10;
    
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
        var response = await CrashClient.Request<Protocol.Response.RoomList>("lobby/room", new Protocol.Request.RoomList
        { });

        // TODO: 기능복구
        UnityEngine.Debug.Log(response);
    }

    public async void OnCreateGameRoom()
    {
        var response = await CrashClient.Request<Protocol.Response.RouteCreate>("lobby/create-room", new Protocol.Request.RouteCreate
        { });

        if (await Client.Instance.Connect(response.Host, (int)response.Port) == false)
        {
            // TODO: 게임서버에 연결못했을 때 에러처리
            return;
        }

        var response1 = await Client.Request<Protocol.Response.Login>(new Login
        {
            Id = Client.Instance.uuid
        });

        var response2 = await Client.Request<Protocol.Response.CreateRoom>(new CreateRoom
        {
            Id = response.Id,
            Title = "my game room title",
            Teams = new System.Collections.Generic.List<int>
            {
                2, 2 // 2 vs 2
            }
        });
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