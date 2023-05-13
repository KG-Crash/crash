using Network;
using Protocol.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KG;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lobby
{
    public class RoomListListener : KG.ScrollView.IListener<string, KG.ButtonSingleTMP>
    {
        public RoomList roomList { get; private set; }
        public BaseClient client { get; private set; }

        public RoomListListener(RoomList response, BaseClient client)
        {
            roomList = response;
            this.client = client;
        }

        public void OnCreated(string data, KG.ButtonSingleTMP button)
        {
            button.text = data;
            button.onClick.AddListener(() => { OnEnterButtonClick(data); });
        }

        private async void OnEnterButtonClick(string RoomId)
        {
            await client.Send(new Protocol.Request.EnterRoom
            {
                Id = RoomId
            });
        }

        public void OnDestroyed(KG.ButtonSingleTMP kgButton)
        {
            kgButton.onClick.RemoveAllListeners();
        }

        public IEnumerator<string> OnRefresh()
        {
            return roomList.Rooms.Select(x => x.Id).GetEnumerator();
        }
    }
}