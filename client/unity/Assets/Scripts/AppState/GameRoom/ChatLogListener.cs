using Protocol.Response;
using System.Collections.Generic;
using UnityEngine.UI;
using KG;

namespace GameRoom
{
    public class ChatLogListener : KG.ScrollView.IListener<string, TextLabel>
    {
        private readonly Chat _response;

        public ChatLogListener(Chat response)
        {
            _response = response;
        }
        public void OnCreated(string data, TextLabel label)
        {
            label.text = data;

            var mine = (data == _response.User);
            if (mine)
                label.textColor = UnityEngine.Color.blue;
        }

        public void OnDestroyed(TextLabel item)
        {
        }

        public IEnumerator<string> OnRefresh()
        {
            return null;
        }
    }
}