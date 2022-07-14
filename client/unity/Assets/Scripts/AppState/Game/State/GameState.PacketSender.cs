using Network;

public partial class GameState
{
    public void SendResume()
    {
        _ = Client.Send(new Protocol.Request.Resume());
    }
}