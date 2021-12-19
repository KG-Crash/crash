using Network;
using Protocol.Response;
using System.Linq;
using System.Threading.Tasks;

namespace Game
{
    public partial class GameController
    {
        [FlatBufferEvent]
        public async Task<bool> OnLeaveRoom(ActionQueue response)
        {
            foreach (var action in response.Actions)
            {
                UnityEngine.Debug.Log($"action frame : {action.Frame}");
                UnityEngine.Debug.Log($"action id : {action.Id}");
                UnityEngine.Debug.Log($"param 1 : {action.Param1}");
                UnityEngine.Debug.Log($"param 2 : {action.Param2}");
            }

            return true;
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
