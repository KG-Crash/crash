using Network;
using Protocol.Request;
using System.Threading.Tasks;

[UI("LobbyView")]
public class LobbyView : UIView
{
    // Start is called before the first frame update
    public KG.ScrollView gameRoomList;

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
        await Client.Send(new CreateRoom { });
    }

    public override async Task OnLoad()
    {
        await Client.Send(new RoomList { });
    }
}
