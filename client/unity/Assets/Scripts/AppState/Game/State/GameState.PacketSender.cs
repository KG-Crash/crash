using Network;

public partial class GameState
{
    public void SendResume()
    {
        _ = Send(new Protocol.Request.Resume());
    }
}