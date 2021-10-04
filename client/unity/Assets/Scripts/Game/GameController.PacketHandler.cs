using Network;
using Protocol.Response;
using System.Threading.Tasks;

namespace Game
{
    public partial class GameController
    {
        [FlatBufferEvent]
        public async Task<bool> OnCreateRoom(CreateRoom response)
        {
            await UIView.Show<GameRoomView>(hideBackView: true);
            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnJoinRoom(JoinRoom response)
        {
            await UIView.Show<GameRoomView>(hideBackView: true);
            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnRoomList(RoomList response)
        {
            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnChat(Chat response)
        {
            UnityEngine.Debug.Log(response.Message);
            return true;
        }
    }
}
