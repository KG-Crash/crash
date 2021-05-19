using Network;
using Protocol.Request;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void OnConnect()
    {
        if (await Client.Instance.Connect("localhost", 8000))
        {
            await Client.Instance.Send(new CreateRoom { });
            await Client.Instance.Send(new JoinRoom { Id = 1001 });
        }
    }
}
