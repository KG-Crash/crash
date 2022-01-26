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
            Debug.Log($"receive queue : {response.Turn}, {response.User}, me?={response.User==Client.Instance.id}");

            if (!_actions.ContainsKey(response.User))
            {
                _actions.Add(response.User, new LinkedList<ActionQueue>());
                Debug.LogError($"OnReady 에서 초기화 안함 : {response.User}");
            }

            var actionQueueList = _actions[response.User];
            var current = actionQueueList.Last; 
            for (var i = actionQueueList.Count - 1; current != null; i++, current = current.Previous)
            {
                var actionQueue = current.Value;
                if (actionQueue.Turn < response.Turn)
                {
                    actionQueueList.AddAfter(current, response);
                    break;
                }
            }

            if (current == null)
            {
                actionQueueList.AddFirst(response);
            }

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
            Debug.Log($"OnReady, myname is {Client.Instance.id}"); 

            foreach (var user in response.Users)
            {
                _actions.Add(user.Id, new LinkedList<ActionQueue>());
                Debug.Log($"OnReady, user.Id={user.Id}");
            }

            return true;
        }
    }
}
