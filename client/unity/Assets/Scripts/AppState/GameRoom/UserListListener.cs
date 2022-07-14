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
    public class UserListListener : KG.ScrollView.IListener<string>
    {
        private string _mine;
        private readonly List<string> _idList = new List<string>();

        public UserListListener(string mine, IEnumerable<string> idList)
        {
            _mine = mine;
            _idList = idList.ToList();
        }

        public void OnCreated(string data, UIBehaviour item)
        {
            var text = item.GetComponent<Text>();
            text.text = data;

            var mine = (data == _mine);
            if (mine)
                text.color = UnityEngine.Color.blue;
        }

        public void OnDestroyed(UIBehaviour item)
        {

        }

        public IEnumerator<string> OnRefresh()
        {
            return _idList.GetEnumerator();
        }
    }
}