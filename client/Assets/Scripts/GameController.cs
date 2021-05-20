using Network;
using Protocol.Response;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Handler.Instance.Bind(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [FlatBufferEvent]
    public bool OnCreateRoom(CreateRoom response)
    {
        return true;
    }

    [FlatBufferEvent]
    public bool OnJoinRoom(JoinRoom response)
    {
        return true;
    }
}
