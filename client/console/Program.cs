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
    public async Task<bool> OnCreateRoom(Protocol.Response.CreateRoom response)
    {
        Console.WriteLine($"게임룸을 생성하여 입장하였음");
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnEnterRoom(Protocol.Response.EnterRoom response)
    {
        if (response.Error > 0)
            return false;

        if (response.User == this.Id)
        {
            Console.WriteLine($"방에 입장함");
        }
        else
        {
            Console.WriteLine($"{response.User}님이 입장하였음");
        }


        // 참여 인원 출력
        Console.WriteLine("참여 인원");
        foreach (var x in response.Users.GroupBy(x => x.Team).Select(x => x))
        {
            Console.WriteLine($"{x.Key}팀");

            foreach (var user in x)
            {
                Console.WriteLine(user.Id);
            }
        }

        await Client.Send(new Protocol.Request.Chat
        {
            Message = "하이요"
        });

        if (response.Master == this.Id)
        {
            //await Client.Send(new Protocol.Request.Whisper
            //{
            //    User = response.User,
            //    Message = "님 그냥 강퇴시킬게요"
            //});

            //await Client.Send(new Protocol.Request.KickRoom
            //{
            //    User = response.User
            //});


            await Client.Send(new Protocol.Request.Whisper
            {
                User = response.User,
                Message = "님 그냥 저 나가볼게요"
            });

            await Client.Send(new Protocol.Request.LeaveRoom
            { });
        }

        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnLeaveRoom(Protocol.Response.LeaveRoom response)
    {
        if (response.Error > 0)
            return false;

        if (response.User == this.Id)
        {
            Console.WriteLine("내가 방을 나감");
        }
        else
        {
            Console.WriteLine($"{response.User}가 퇴장");
        }

        // 방장 변경됨
        if (string.IsNullOrEmpty(response.NewMaster) == false)
        {
            Console.WriteLine($"{response.NewMaster}님이 새로운 방장이 됨");
        }

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
            await Client.Send(new Protocol.Request.EnterRoom
            {
                Id = response.Rooms.First().Id
            });
        }
        else
        {
            await Client.Send(new Protocol.Request.CreateRoom 
            {
                Title = "my game room title",
                Teams = new System.Collections.Generic.List<int>
                {
                    2, 2 // 2 vs 2
                }
            });
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

        var received = (response.To == this.Id);
        if (received)
        {
            Console.WriteLine($"{response.From} >> {response.Message}");
        }
        else
        {
            Console.WriteLine($"{response.To} << {response.Message}");
        }

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
