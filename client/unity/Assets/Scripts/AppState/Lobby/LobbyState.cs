using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using Network;
using UnityEngine;
using System.Threading.Tasks;
using Protocol.Request;
using UI;

[UIBind(typeof(LobbyPanel), true)]
[AutoBind(flatBuffer: true)]
public partial class LobbyState : AppState
{
    private DateTime _lastUpdatedRefreshDate = DateTime.Now;
    public const int REFRESH_ROOM_INTERVAL = 10;
    
    [InitializeMethod]
    public void Initialize()
    {
        var view = GetView<LobbyPanel>();
        view.createGameRoomButtonClick.AddListener(OnCreateGameRoom);
        _ = Refresh();
        
        StartCoroutine(RefreshRoom(REFRESH_ROOM_INTERVAL));
    }

    [ClearMethod]
    public void Clear()
    {
        var view = GetView<LobbyPanel>();
        view.createGameRoomButtonClick.RemoveListener(OnCreateGameRoom);
    }
    
    public async Task Refresh() => await Client.Send(new RoomList { });

    public async void OnCreateGameRoom()
    {
        await Client.Send(new CreateRoom 
        {
            Title = "my game room title",
            Teams = new System.Collections.Generic.List<int>
            {
                2, 2 // 2 vs 2
            }
        });
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