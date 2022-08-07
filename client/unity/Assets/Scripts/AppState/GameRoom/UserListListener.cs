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
    public class UserListListener : KG.ScrollView.IListener<string, TextLabel>
    {
        private string _mine;
        private readonly List<string> _idList = new List<string>();

        public UserListListener(string mine, IEnumerable<string> idList)
        {
            _mine = mine;
            _idList = idList.ToList();
        }

        public void OnCreated(string data, TextLabel label)
        {
            label.text = data;

            var mine = (data == _mine);
            if (mine)
                label.textColor = UnityEngine.Color.blue;
        }

        public void OnDestroyed(TextLabel item)
        {
        }

        public IEnumerator<string> OnRefresh()
        {
            return _idList.GetEnumerator();
        }
    }
}