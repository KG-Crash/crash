#define CASE_KICK

using Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = "http://127.0.0.1:8001";
            var client = new Client();
            await client.Request<Protocol.Response.Authentication>(host, "authentication", new Protocol.Request.Authentication
            {
                Id = client.Id
            });

            var roomList = await client.Request<Protocol.Response.RoomList>(host, "lobby/room", new Protocol.Request.RoomList());
            if (roomList.Rooms.Count == 0)
            {
                var routeCreate = await client.Request<Protocol.Response.RouteCreate>(host, "lobby/create-room", new Protocol.Request.RouteCreate());
                if (await client.Connect(routeCreate.Host, (int)routeCreate.Port) == false)
                    throw new Exception($"cannot connect to host {routeCreate.Host}:{routeCreate.Port}");

                await client.Request<Protocol.Response.Login>(new Protocol.Request.Login
                {
                    Id = routeCreate.Id
                });

                var createRoom = await client.Request<Protocol.Response.CreateRoom>(new Protocol.Request.CreateRoom
                { 
                    Id = routeCreate.Id,
                    Teams = new List<int> { 2, 2 },
                    Title = routeCreate.Id
                });
                if (createRoom.Error != 0)
                    throw new Exception("cannot create game room");

                await client.Request<Protocol.Response.LeaveRoom>(new Protocol.Request.LeaveRoom());
            }
        }
    }
}
