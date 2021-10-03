using Network;
using Protocol.Request;

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

    public async void OnConnect()
    {
        if (await Client.Instance.Connect("localhost", 8000))
        {
            await Client.Instance.Send(new CreateRoom { });
            await Client.Instance.Send(new JoinRoom { Id = "hello" });
        }
    }
}
