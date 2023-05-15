public class GameRoomTransition : StateTransition
{
    public string Host { get; set; }
    public uint Port { get; set; }
    public bool Enter { get; set; }
    public string RoomId { get; set; }

    public Protocol.Response.RouteCreate RouteCreate { get; set; }
    public Protocol.Response.RouteEnter RouteEnter { get; set; }
    
    // public bool isMine { get; private set; }
    // public string[] roomUsers { get; private set; }

    // public GameRoomTransition(bool isMine, string[] roomUsers)
    // {
    //     this.isMine = isMine;
    //     this.roomUsers = roomUsers;
    // }
}