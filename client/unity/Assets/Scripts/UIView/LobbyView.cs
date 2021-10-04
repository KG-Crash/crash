using Network;
using Protocol.Request;
using System.Threading.Tasks;

[UI("LobbyView")]
public class LobbyView : UIView
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public async void OnCreateGameRoom()
    {
        await Client.Instance.Send(new CreateRoom { });
    }

    public override async Task OnLoad()
    {
        await Client.Instance.Send(new RoomList { });
    }
}
