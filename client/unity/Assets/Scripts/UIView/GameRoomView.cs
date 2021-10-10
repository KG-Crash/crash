using Network;
using Protocol.Request;
using System.Threading.Tasks;

[UI("GameRoomView")]
public class GameRoomView : UIView
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
        await Client.Send(new Chat 
        {
            Message = "¾È³ç" 
        });
    }
}
