using Network;
using Protocol.Response;
using System.Linq;

namespace Game
{
    public partial class GameController
    {
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
}
