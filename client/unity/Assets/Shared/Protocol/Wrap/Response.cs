using System.Collections.Generic;
using System.Linq;

namespace Protocol.Response
{
    public enum Identity
    {
        ROOM,
        LOGIN,
        CREATE_ROOM,
        USER,
        ENTER_ROOM,
        LEAVE_ROOM,
        KICK_ROOM,
        KICKED_ROOM,
        DESTROYED_ROOM,
        ROOM_LIST,
        CHAT,
        WHISPER,
        IN_GAME_CHAT,
        RESUME,
        ACTION,
        ACTION_QUEUE,
        TEAM,
        GAME_START,
        READY
    }

    public class Room : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.ROOM;

        public string Id { get; set; }
        public string Title { get; set; }

        public Room()
        { }

        public Room(FlatBuffer.Response.Room obj)
        {
            this.Id = obj.Id;
            this.Title = obj.Title;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.Room> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _id = builder.CreateString(this.Id);
            var _title = builder.CreateString(this.Title);

            return FlatBuffer.Response.Room.CreateRoom(builder, _id, _title);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Room Deserialize(byte[] bytes)
        {
            return new Room(FlatBuffer.Response.Room.GetRootAsRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Login : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.LOGIN;

        public string Id { get; set; }
        public uint Error { get; set; }

        public Login()
        { }

        public Login(FlatBuffer.Response.Login obj)
        {
            this.Id = obj.Id;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.Login> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _id = builder.CreateString(this.Id);
            var _error = this.Error;

            return FlatBuffer.Response.Login.CreateLogin(builder, _id, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Login Deserialize(byte[] bytes)
        {
            return new Login(FlatBuffer.Response.Login.GetRootAsLogin(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class CreateRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.CREATE_ROOM;

        public string Id { get; set; }
        public uint Error { get; set; }

        public CreateRoom()
        { }

        public CreateRoom(FlatBuffer.Response.CreateRoom obj)
        {
            this.Id = obj.Id;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.CreateRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _id = builder.CreateString(this.Id);
            var _error = this.Error;

            return FlatBuffer.Response.CreateRoom.CreateCreateRoom(builder, _id, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static CreateRoom Deserialize(byte[] bytes)
        {
            return new CreateRoom(FlatBuffer.Response.CreateRoom.GetRootAsCreateRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class User : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.USER;

        public string Id { get; set; }
        public int Team { get; set; }
        public int Sequence { get; set; }

        public User()
        { }

        public User(FlatBuffer.Response.User obj)
        {
            this.Id = obj.Id;
            this.Team = obj.Team;
            this.Sequence = obj.Sequence;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.User> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _id = builder.CreateString(this.Id);
            var _team = this.Team;
            var _sequence = this.Sequence;

            return FlatBuffer.Response.User.CreateUser(builder, _id, _team, _sequence);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static User Deserialize(byte[] bytes)
        {
            return new User(FlatBuffer.Response.User.GetRootAsUser(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class EnterRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.ENTER_ROOM;

        public string User { get; set; }
        public List<User> Users { get; set; }
        public string Master { get; set; }
        public uint Error { get; set; }

        public EnterRoom()
        { }

        public EnterRoom(FlatBuffer.Response.EnterRoom obj)
        {
            this.User = obj.User;
            this.Users = Enumerable.Range(0, obj.UsersLength).Select(x => new User(obj.Users(x).Value)).ToList();
            this.Master = obj.Master;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.EnterRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _user = builder.CreateString(this.User);
            var _users = FlatBuffer.Response.EnterRoom.CreateUsersVector(builder, this.Users.Select(x => x.ToFlatBuffer(builder)).ToArray());
            var _master = builder.CreateString(this.Master);
            var _error = this.Error;

            return FlatBuffer.Response.EnterRoom.CreateEnterRoom(builder, _user, _users, _master, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static EnterRoom Deserialize(byte[] bytes)
        {
            return new EnterRoom(FlatBuffer.Response.EnterRoom.GetRootAsEnterRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class LeaveRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.LEAVE_ROOM;

        public string User { get; set; }
        public string NewMaster { get; set; }
        public uint Error { get; set; }

        public LeaveRoom()
        { }

        public LeaveRoom(FlatBuffer.Response.LeaveRoom obj)
        {
            this.User = obj.User;
            this.NewMaster = obj.NewMaster;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.LeaveRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _user = builder.CreateString(this.User);
            var _newMaster = builder.CreateString(this.NewMaster);
            var _error = this.Error;

            return FlatBuffer.Response.LeaveRoom.CreateLeaveRoom(builder, _user, _newMaster, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static LeaveRoom Deserialize(byte[] bytes)
        {
            return new LeaveRoom(FlatBuffer.Response.LeaveRoom.GetRootAsLeaveRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class KickRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.KICK_ROOM;

        public bool Success { get; set; }
        public uint Error { get; set; }

        public KickRoom()
        { }

        public KickRoom(FlatBuffer.Response.KickRoom obj)
        {
            this.Success = obj.Success;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.KickRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _success = this.Success;
            var _error = this.Error;

            return FlatBuffer.Response.KickRoom.CreateKickRoom(builder, _success, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static KickRoom Deserialize(byte[] bytes)
        {
            return new KickRoom(FlatBuffer.Response.KickRoom.GetRootAsKickRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class KickedRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.KICKED_ROOM;

        public uint Error { get; set; }

        public KickedRoom()
        { }

        public KickedRoom(FlatBuffer.Response.KickedRoom obj)
        {
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.KickedRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _error = this.Error;

            return FlatBuffer.Response.KickedRoom.CreateKickedRoom(builder, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static KickedRoom Deserialize(byte[] bytes)
        {
            return new KickedRoom(FlatBuffer.Response.KickedRoom.GetRootAsKickedRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class DestroyedRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.DESTROYED_ROOM;

        public uint Error { get; set; }

        public DestroyedRoom()
        { }

        public DestroyedRoom(FlatBuffer.Response.DestroyedRoom obj)
        {
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.DestroyedRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _error = this.Error;

            return FlatBuffer.Response.DestroyedRoom.CreateDestroyedRoom(builder, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static DestroyedRoom Deserialize(byte[] bytes)
        {
            return new DestroyedRoom(FlatBuffer.Response.DestroyedRoom.GetRootAsDestroyedRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class RoomList : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.ROOM_LIST;

        public List<Room> Rooms { get; set; }
        public uint Error { get; set; }

        public RoomList()
        { }

        public RoomList(FlatBuffer.Response.RoomList obj)
        {
            this.Rooms = Enumerable.Range(0, obj.RoomsLength).Select(x => new Room(obj.Rooms(x).Value)).ToList();
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.RoomList> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _rooms = FlatBuffer.Response.RoomList.CreateRoomsVector(builder, this.Rooms.Select(x => x.ToFlatBuffer(builder)).ToArray());
            var _error = this.Error;

            return FlatBuffer.Response.RoomList.CreateRoomList(builder, _rooms, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static RoomList Deserialize(byte[] bytes)
        {
            return new RoomList(FlatBuffer.Response.RoomList.GetRootAsRoomList(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Chat : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.CHAT;

        public string User { get; set; }
        public string Message { get; set; }
        public uint Error { get; set; }

        public Chat()
        { }

        public Chat(FlatBuffer.Response.Chat obj)
        {
            this.User = obj.User;
            this.Message = obj.Message;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.Chat> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _user = builder.CreateString(this.User);
            var _message = builder.CreateString(this.Message);
            var _error = this.Error;

            return FlatBuffer.Response.Chat.CreateChat(builder, _user, _message, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Chat Deserialize(byte[] bytes)
        {
            return new Chat(FlatBuffer.Response.Chat.GetRootAsChat(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Whisper : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.WHISPER;

        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public uint Error { get; set; }

        public Whisper()
        { }

        public Whisper(FlatBuffer.Response.Whisper obj)
        {
            this.From = obj.From;
            this.To = obj.To;
            this.Message = obj.Message;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.Whisper> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _from = builder.CreateString(this.From);
            var _to = builder.CreateString(this.To);
            var _message = builder.CreateString(this.Message);
            var _error = this.Error;

            return FlatBuffer.Response.Whisper.CreateWhisper(builder, _from, _to, _message, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Whisper Deserialize(byte[] bytes)
        {
            return new Whisper(FlatBuffer.Response.Whisper.GetRootAsWhisper(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class InGameChat : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.IN_GAME_CHAT;

        public int Turn { get; set; }
        public int Frame { get; set; }
        public int User { get; set; }
        public string Message { get; set; }
        public uint Error { get; set; }

        public InGameChat()
        { }

        public InGameChat(FlatBuffer.Response.InGameChat obj)
        {
            this.Turn = obj.Turn;
            this.Frame = obj.Frame;
            this.User = obj.User;
            this.Message = obj.Message;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.InGameChat> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _turn = this.Turn;
            var _frame = this.Frame;
            var _user = this.User;
            var _message = builder.CreateString(this.Message);
            var _error = this.Error;

            return FlatBuffer.Response.InGameChat.CreateInGameChat(builder, _turn, _frame, _user, _message, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static InGameChat Deserialize(byte[] bytes)
        {
            return new InGameChat(FlatBuffer.Response.InGameChat.GetRootAsInGameChat(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Resume : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.RESUME;

        public string User { get; set; }
        public uint Error { get; set; }

        public Resume()
        { }

        public Resume(FlatBuffer.Response.Resume obj)
        {
            this.User = obj.User;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.Resume> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _user = builder.CreateString(this.User);
            var _error = this.Error;

            return FlatBuffer.Response.Resume.CreateResume(builder, _user, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Resume Deserialize(byte[] bytes)
        {
            return new Resume(FlatBuffer.Response.Resume.GetRootAsResume(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Action : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.ACTION;

        public int Frame { get; set; }
        public int Id { get; set; }
        public uint Param1 { get; set; }
        public uint Param2 { get; set; }

        public Action()
        { }

        public Action(FlatBuffer.Response.Action obj)
        {
            this.Frame = obj.Frame;
            this.Id = obj.Id;
            this.Param1 = obj.Param1;
            this.Param2 = obj.Param2;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.Action> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _frame = this.Frame;
            var _id = this.Id;
            var _param1 = this.Param1;
            var _param2 = this.Param2;

            return FlatBuffer.Response.Action.CreateAction(builder, _frame, _id, _param1, _param2);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Action Deserialize(byte[] bytes)
        {
            return new Action(FlatBuffer.Response.Action.GetRootAsAction(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class ActionQueue : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.ACTION_QUEUE;

        public int User { get; set; }
        public List<Action> Actions { get; set; }
        public int Turn { get; set; }
        public uint Error { get; set; }

        public ActionQueue()
        { }

        public ActionQueue(FlatBuffer.Response.ActionQueue obj)
        {
            this.User = obj.User;
            this.Actions = Enumerable.Range(0, obj.ActionsLength).Select(x => new Action(obj.Actions(x).Value)).ToList();
            this.Turn = obj.Turn;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.ActionQueue> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _user = this.User;
            var _actions = FlatBuffer.Response.ActionQueue.CreateActionsVector(builder, this.Actions.Select(x => x.ToFlatBuffer(builder)).ToArray());
            var _turn = this.Turn;
            var _error = this.Error;

            return FlatBuffer.Response.ActionQueue.CreateActionQueue(builder, _user, _actions, _turn, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static ActionQueue Deserialize(byte[] bytes)
        {
            return new ActionQueue(FlatBuffer.Response.ActionQueue.GetRootAsActionQueue(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Team : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.TEAM;

        public List<string> Users { get; set; }

        public Team()
        { }

        public Team(FlatBuffer.Response.Team obj)
        {
            this.Users = Enumerable.Range(0, obj.UsersLength).Select(x => obj.Users(x)).ToList();
        }

        public FlatBuffers.Offset<FlatBuffer.Response.Team> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _users = FlatBuffer.Response.Team.CreateUsersVector(builder, this.Users.Select(x => builder.CreateString(x)).ToArray());

            return FlatBuffer.Response.Team.CreateTeam(builder, _users);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Team Deserialize(byte[] bytes)
        {
            return new Team(FlatBuffer.Response.Team.GetRootAsTeam(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class GameStart : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.GAME_START;

        public uint Error { get; set; }

        public GameStart()
        { }

        public GameStart(FlatBuffer.Response.GameStart obj)
        {
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.GameStart> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _error = this.Error;

            return FlatBuffer.Response.GameStart.CreateGameStart(builder, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static GameStart Deserialize(byte[] bytes)
        {
            return new GameStart(FlatBuffer.Response.GameStart.GetRootAsGameStart(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Ready : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.READY;

        public long Seed { get; set; }
        public List<User> Users { get; set; }
        public uint Error { get; set; }

        public Ready()
        { }

        public Ready(FlatBuffer.Response.Ready obj)
        {
            this.Seed = obj.Seed;
            this.Users = Enumerable.Range(0, obj.UsersLength).Select(x => new User(obj.Users(x).Value)).ToList();
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.Ready> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _seed = this.Seed;
            var _users = FlatBuffer.Response.Ready.CreateUsersVector(builder, this.Users.Select(x => x.ToFlatBuffer(builder)).ToArray());
            var _error = this.Error;

            return FlatBuffer.Response.Ready.CreateReady(builder, _seed, _users, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Ready Deserialize(byte[] bytes)
        {
            return new Ready(FlatBuffer.Response.Ready.GetRootAsReady(new FlatBuffers.ByteBuffer(bytes)));
        }
    }
}