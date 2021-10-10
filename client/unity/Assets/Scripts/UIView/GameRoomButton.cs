using Network;
using UnityEngine;

public class GameRoomButton : MonoBehaviour
{
    public string RoomId { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void OnJoin()
    {
        await Client.Send(new Protocol.Request.JoinRoom
        {
            Id = RoomId
        });
    }
}
