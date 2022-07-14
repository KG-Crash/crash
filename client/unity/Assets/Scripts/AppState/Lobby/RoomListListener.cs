using Network;
using Protocol.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lobby
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
            return roomList.Rooms.Select(x => x.Id).GetEnumerator();
        }
    }
}