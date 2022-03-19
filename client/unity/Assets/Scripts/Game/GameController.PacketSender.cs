using Network;

namespace Game
{
    public partial class GameController
    {
        public void SendResume()
        {
            _ = Client.Send(new Protocol.Request.Resume());
        }
    }
}