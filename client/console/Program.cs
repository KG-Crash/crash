using Network;
using System;
using System.Threading.Tasks;

public class Controller
{
    public Controller()
    {
        Handler.Instance.Bind(this);
    }

    [FlatBufferEvent]
    public bool OnCreateRoom(Protocol.Response.CreateRoom response)
    {
        return true;
    }

    [FlatBufferEvent]
    public bool OnJoinRoom(Protocol.Response.JoinRoom response)
    {
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
                await Client.Instance.Send(new Protocol.Request.CreateRoom { });
                await Client.Instance.Send(new Protocol.Request.JoinRoom { Id = "hello" });
            }

            Console.ReadLine();
        }
    }
}
