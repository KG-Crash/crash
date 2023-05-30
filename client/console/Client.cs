using Network;
using System;
using System.Threading.Tasks;

namespace console
{
    public class Client : BaseClient
    {
        public string Id { get; private set; }
        public bool IsMaster { get; private set; }

        public Client()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Client(string uuid)
        {
            Id = uuid;
        }

        [FlatBufferEvent]
        public async Task<bool> OnAuthentication(Protocol.Response.Authentication response)
        {
            if (response.Error > 0)
                return false;

            Token = response.Token;
            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnLogin(Protocol.Response.Login response)
        {
            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnCreateRoom(Protocol.Response.CreateRoom response)
        {
            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnLeaveRoom(Protocol.Response.LeaveRoom response)
        {
            return true;
        }
    }
}
