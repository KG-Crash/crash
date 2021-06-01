using Network;
using System;
using System.Linq;
using System.Threading.Tasks;

public class Controller
{
    public string Id { get; private set; }

    public Controller()
    {
        Handler.Instance.Bind(this);
    }

    [FlatBufferEvent]
    public bool OnLogin(Protocol.Response.Login response)
    {
        if (response.Error > 0)
            return false;

        this.Id = response.Id;
        return true;
    }

    [FlatBufferEvent]
    public bool OnJoinRoom(Protocol.Response.JoinRoom response)
    {
        if (response.Error > 0)
            return false;

        if (response.User == this.Id)
        {
            _ = Client.Instance.Send(new Protocol.Request.Chat
            {
                Message = "ㅆ1발ㅋㅋ"
            });
        }
        else
        {
            _ = Client.Instance.Send(new Protocol.Request.Whisper
            {
                User = response.User,
                Message = "ㅆ1발련 ㅋㅋ"
            });
        }

        return true;
    }

    [FlatBufferEvent]
    public bool OnLeaveRoom(Protocol.Response.LeaveRoom response)
    {
        if (response.Error > 0)
            return false;

        Console.WriteLine($"{response.User}가 나갔음.");
        return true;
    }

    [FlatBufferEvent]
    public bool OnDestroyRoom(Protocol.Response.DestroyedRoom response)
    {
        if (response.Error > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [FlatBufferEvent]
    public bool OnRoomList(Protocol.Response.RoomList response)
    {
        if (response.Error > 0)
            return false;


        if (response.Rooms.Count > 0)
        {
            _ = Client.Instance.Send(new Protocol.Request.JoinRoom
            {
                Id = response.Rooms.First()
            });
        }
        else
        {
            _ = Client.Instance.Send(new Protocol.Request.CreateRoom { });
        }

        return true;
    }

    [FlatBufferEvent]
    public bool OnChat(Protocol.Response.Chat response)
    {
        if (response.Error > 0)
            return false;

        Console.WriteLine($"{response.User} : {response.Message}");
        return true;
    }

    [FlatBufferEvent]
    public bool OnWhisper(Protocol.Response.Whisper response)
    {
        if (response.Error > 0)
            return false;

        Console.WriteLine($"{response.User}로부터 다음의 귓속말을 받음 : {response.Message}");
        return true;
    }
}

namespace console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var controller = new Controller();

            if (await Client.Instance.Connect("localhost", 8000))
            {
                await Client.Instance.Send(new Protocol.Request.RoomList { });
            }

            Console.ReadLine();
        }
    }
}
