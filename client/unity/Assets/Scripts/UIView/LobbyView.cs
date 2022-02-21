using Network;
using Protocol.Request;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[UI("LobbyView")]
public class LobbyView : UIView
{
    public const int REFRESH_ROOM_INTERVAL = 10;

    private DateTime _lastUpdatedRefreshDate = DateTime.Now;

    // Start is called before the first frame update
    public KG.ScrollView gameRoomList;

    public override void Start()
    {
        base.Start();

        StartCoroutine(RefreshRoom(REFRESH_ROOM_INTERVAL));
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

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

    public override async Task OnLoad()
    {
        await Refresh();
    }

    public async Task Refresh() => await Client.Send(new RoomList { });

    public IEnumerator RefreshRoom(int seconds) 
    {
        while (true)
        {
            var elapsedTime = DateTime.Now - _lastUpdatedRefreshDate;
            if (elapsedTime.TotalSeconds < REFRESH_ROOM_INTERVAL)
            {
                yield return new WaitForSeconds(REFRESH_ROOM_INTERVAL - (int)elapsedTime.TotalSeconds);
            }
            else
            {
                yield return new WaitForSeconds(REFRESH_ROOM_INTERVAL);
            }

            _lastUpdatedRefreshDate = DateTime.Now;
            _ = Refresh();
            UnityEngine.Debug.Log("request room list");
        }
    }
}
