public class GameRoomTransition : StateTransition
{
    public string Host { get; set; }
    public uint Port { get; set; }
    public bool Enter { get; set; }
    public string RoomId { get; set; }
}