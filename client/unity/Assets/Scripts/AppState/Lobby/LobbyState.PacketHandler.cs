using Lobby;
using Network;
using Protocol.Response;
using System.Linq;
using System.Threading.Tasks;
using UI;
#pragma warning disable 1998

public partial class LobbyState
{
    
    [FlatBufferEvent]
    public async Task<bool> OnRoomList(RoomList response)
    {
        GetView<LobbyPanel>().gameRoomList.Refresh(new RoomListListener(response, this));
        return true;
    }
}