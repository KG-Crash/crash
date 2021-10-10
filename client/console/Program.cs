using Network;
using Protocol.Request;
using System;
using System.Linq;
using System.Threading.Tasks;

public class Controller
{
    public string Id { get; private set; }
    public bool IsMaster { get; private set; }

    public Controller()
    {
        Handler.Bind(this);
    }

    [FlatBufferEvent]
    public async Task<bool> OnLogin(Protocol.Response.Login response)
    {
        if (response.Error > 0)
            return false;

        this.Id = response.Id;
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnJoinRoom(Protocol.Response.JoinRoom response)
    {
        if (response.Error > 0)
            return false;

        this.IsMaster = response.Master;

        if (response.User == this.Id)
        {
            await Client.Send(new Protocol.Request.Chat
            {
                Message = "ㅆ1발ㅋㅋ"
            });
        }
        else
        {
            await Client.Send(new Protocol.Request.Whisper
            {
                User = response.User,
                Message = "ㅆ1발련 ㅋㅋ"
            });
        }

        // 내가 방장이면 오자마자 바로강퇴시킴
        //await Client.Send(new Protocol.Request.KickRoom
        //{ 
        //    User = response.User
        //});

        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnLeaveRoom(Protocol.Response.LeaveRoom response)
    {
        if (response.Error > 0)
            return false;

        Console.WriteLine($"{response.User}가 나갔음.");
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnDestroyRoom(Protocol.Response.DestroyedRoom response)
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
    public async Task<bool> OnRoomList(Protocol.Response.RoomList response)
    {
        if (response.Error > 0)
            return false;


        if (response.Rooms.Count > 0)
        {
            await Client.Send(new Protocol.Request.JoinRoom
            {
                Id = response.Rooms.First()
            });
        }
        else
        {
            await Client.Send(new Protocol.Request.CreateRoom { });
        }

        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnChat(Protocol.Response.Chat response)
    {
        if (response.Error > 0)
            return false;

        Console.WriteLine($"{response.User} : {response.Message}");
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnWhisper(Protocol.Response.Whisper response)
    {
        if (response.Error > 0)
            return false;

        Console.WriteLine($"{response.User}로부터 다음의 귓속말을 받음 : {response.Message}");
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnKicked(Protocol.Response.KickedRoom response)
    { 
        if(response.Error > 0)
            return false;

        Console.WriteLine("당신은 강퇴당했습니다.");
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
                await Client.Send(new RoomList { });
            }

            Console.ReadLine();
        }
    }
}
