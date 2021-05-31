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
    public bool OnJoinRoom(Protocol.Response.JoinRoom response)
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
    public bool OnLeaveRoom(Protocol.Response.LeaveRoom response)
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
                await Client.Instance.Send(new Protocol.Request.LeaveRoom { });
            }

            Console.ReadLine();
        }
    }
}
