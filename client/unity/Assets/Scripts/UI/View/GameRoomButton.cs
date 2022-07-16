using Network;
using UnityEngine;

public class GameRoomButton : MonoBehaviour
{
    public string RoomId { get; set; }

    public async void OnEnter()
    {
        await Client.Send(new Protocol.Request.EnterRoom
        {
            Id = RoomId
        });
    }
}
