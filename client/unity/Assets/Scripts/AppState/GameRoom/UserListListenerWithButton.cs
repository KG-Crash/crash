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

namespace GameRoom
{
    public class UserListListenerWithButton : KG.ScrollView.IListener<string, ButtonSingleTMP>
    {
        private string _mine;
        private readonly List<string> _idList = new List<string>();

        public UserListListenerWithButton(string mine, IEnumerable<string> idList)
        {
            _mine = mine;
            _idList = idList.ToList();
        }

        public void OnCreated(string data, ButtonSingleTMP button)
        {
            button.text = data;

            var mine = (data == _mine);
            if (mine)
                button.color = UnityEngine.Color.blue;
        }

        public void OnDestroyed(ButtonSingleTMP item)
        {
        }

        public IEnumerator<string> OnRefresh()
        {
            return _idList.GetEnumerator();
        }
    }
}