using System.Collections.Generic;
using Network;
using Protocol.Response;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public partial class GameController
    {
        private Dictionary<string, LinkedList<ActionQueue>> _actions = new Dictionary<string, LinkedList<ActionQueue>>();
        
        [FlatBufferEvent]
        public async Task<bool> OnActionQueue(ActionQueue response)
        {
            Debug.Log($"turn : {response.Actions[0].Param1}");

            if (!_actions.ContainsKey(response.User))
            {
                _actions.Add(response.User, new LinkedList<ActionQueue>());
                Debug.LogError($"초기화 되지 않은 유저 이름 : {response.User}");
                return false;
            }

            // 프레임 수 정렬 보장 필요
            _actions[response.User].AddLast(response);
            return true;
        }

        public void OnActionSelf(ActionQueue myAction)
        {
            
        }
        
        [FlatBufferEvent]
        public async Task<bool> OnReady(Ready response)
        {
            // team : users
            var users = response.Users
                .GroupBy(x => x.Team)
                .ToDictionary(x => x.Key, x => x.ToList());

            // random seed
            Client.Instance.seed = response.Seed;

            return true;
        }
    }
}
