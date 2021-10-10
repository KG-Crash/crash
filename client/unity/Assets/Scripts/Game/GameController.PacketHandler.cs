using Network;
using Protocol.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class RoomListListener : KG.ScrollView.IListener<string>
    {
        public RoomList roomList { get; private set; }

        public RoomListListener(RoomList response)
        {
            roomList = response;
        }

        public void OnCreated(string data, UIBehaviour item)
        {
            item.GetComponent<GameRoomButton>().RoomId = data;

            var button = item.GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = data;
        }

        public void OnDestroyed(UIBehaviour item)
        { }

        public IEnumerator<string> OnRefresh()
        {
            return roomList.Rooms.GetEnumerator();
        }
    }

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
            var uiView = UIView.Get<LobbyView>();
            var listener = new RoomListListener(response);
            uiView.gameRoomList.Refresh(listener);
            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnChat(Chat response)
        {
            UnityEngine.Debug.Log(response.Message);
            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnWhisper(Whisper response)
        {
            UnityEngine.Debug.Log($"{response.User} : {response.Message}");
            return true;
        }
    }
}
