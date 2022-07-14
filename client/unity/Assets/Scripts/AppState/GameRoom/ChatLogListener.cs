using Network;
using Protocol.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameRoom
{
    public class ChatLogListener : KG.ScrollView.IListener<string>
    {
        private readonly Chat _response;

        public ChatLogListener(Chat response)
        {
            _response = response;
        }
        public void OnCreated(string data, UIBehaviour item)
        {
            var text = item.GetComponent<Text>();
            text.text = data;

            var mine = (data == _response.User);
            if (mine)
                text.color = UnityEngine.Color.blue;
        }

        public void OnDestroyed(UIBehaviour item)
        {

        }

        public IEnumerator<string> OnRefresh()
        {
            return null;
        }
    }
}