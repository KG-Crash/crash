using Network;
using Protocol.Request;
using System.Threading.Tasks;

[UI("IntroView")]
public class IntroView : UIView
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

    public override async Task OnLoad()
    {
        await Client.Instance.Disconnect();
    }

    public async void OnConnect()
    {
        if (await Client.Instance.Connect("localhost", 8000))
        {
            //await Client.Instance.Send(new CreateRoom { });
            //await Client.Instance.Send(new JoinRoom { Id = "hello" });

            await UIView.Show<LobbyView>(hideBackView: true);
        }
    }
}
