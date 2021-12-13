using System.Collections.Generic;
using System.Linq;

namespace Protocol.Request
{
    public enum Identity
    {
        CREATE_ROOM,
        ENTER_ROOM,
        LEAVE_ROOM,
        KICK_ROOM,
        ROOM_LIST,
        CHAT,
        WHISPER,
        ACTION,
        ACTION_QUEUE,
        GAME_START
    }

    public class CreateRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.CREATE_ROOM;

        public string Title { get; set; }
        public List<int> Teams { get; set; }

        public CreateRoom()
        { }

        public CreateRoom(FlatBuffer.Request.CreateRoom obj)
        {
            this.Title = obj.Title;
            this.Teams = Enumerable.Range(0, obj.TeamsLength).Select(x => obj.Teams(x)).ToList();
        }

        public FlatBuffers.Offset<FlatBuffer.Request.CreateRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _title = builder.CreateString(this.Title);
            var _teams = FlatBuffer.Request.CreateRoom.CreateTeamsVector(builder, this.Teams.ToArray());

            return FlatBuffer.Request.CreateRoom.CreateCreateRoom(builder, _title, _teams);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static CreateRoom Deserialize(byte[] bytes)
        {
            return new CreateRoom(FlatBuffer.Request.CreateRoom.GetRootAsCreateRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class EnterRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.ENTER_ROOM;

        public string Id { get; set; }

        public EnterRoom()
        { }

        public EnterRoom(FlatBuffer.Request.EnterRoom obj)
        {
            this.Id = obj.Id;
        }

        public FlatBuffers.Offset<FlatBuffer.Request.EnterRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _id = builder.CreateString(this.Id);

            return FlatBuffer.Request.EnterRoom.CreateEnterRoom(builder, _id);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static EnterRoom Deserialize(byte[] bytes)
        {
            return new EnterRoom(FlatBuffer.Request.EnterRoom.GetRootAsEnterRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class LeaveRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.LEAVE_ROOM;

        

        public LeaveRoom()
        { }

        public LeaveRoom(FlatBuffer.Request.LeaveRoom obj)
        {
            
        }

        public FlatBuffers.Offset<FlatBuffer.Request.LeaveRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            FlatBuffer.Request.LeaveRoom.StartLeaveRoom(builder);
            return FlatBuffer.Request.LeaveRoom.EndLeaveRoom(builder);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static LeaveRoom Deserialize(byte[] bytes)
        {
            return new LeaveRoom(FlatBuffer.Request.LeaveRoom.GetRootAsLeaveRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class KickRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.KICK_ROOM;

        public string User { get; set; }

        public KickRoom()
        { }

        public KickRoom(FlatBuffer.Request.KickRoom obj)
        {
            this.User = obj.User;
        }

        public FlatBuffers.Offset<FlatBuffer.Request.KickRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _user = builder.CreateString(this.User);

            return FlatBuffer.Request.KickRoom.CreateKickRoom(builder, _user);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static KickRoom Deserialize(byte[] bytes)
        {
            return new KickRoom(FlatBuffer.Request.KickRoom.GetRootAsKickRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class RoomList : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.ROOM_LIST;

        

        public RoomList()
        { }

        public RoomList(FlatBuffer.Request.RoomList obj)
        {
            
        }

        public FlatBuffers.Offset<FlatBuffer.Request.RoomList> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            FlatBuffer.Request.RoomList.StartRoomList(builder);
            return FlatBuffer.Request.RoomList.EndRoomList(builder);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static RoomList Deserialize(byte[] bytes)
        {
            return new RoomList(FlatBuffer.Request.RoomList.GetRootAsRoomList(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Chat : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.CHAT;

        public string Message { get; set; }

        public Chat()
        { }

        public Chat(FlatBuffer.Request.Chat obj)
        {
            this.Message = obj.Message;
        }

        public FlatBuffers.Offset<FlatBuffer.Request.Chat> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _message = builder.CreateString(this.Message);

            return FlatBuffer.Request.Chat.CreateChat(builder, _message);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Chat Deserialize(byte[] bytes)
        {
            return new Chat(FlatBuffer.Request.Chat.GetRootAsChat(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Whisper : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.WHISPER;

        public string User { get; set; }
        public string Message { get; set; }

        public Whisper()
        { }

        public Whisper(FlatBuffer.Request.Whisper obj)
        {
            this.User = obj.User;
            this.Message = obj.Message;
        }

        public FlatBuffers.Offset<FlatBuffer.Request.Whisper> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _user = builder.CreateString(this.User);
            var _message = builder.CreateString(this.Message);

            return FlatBuffer.Request.Whisper.CreateWhisper(builder, _user, _message);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Whisper Deserialize(byte[] bytes)
        {
            return new Whisper(FlatBuffer.Request.Whisper.GetRootAsWhisper(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class Action : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.ACTION;

        public int Id { get; set; }
        public int Frame { get; set; }
        public uint Param1 { get; set; }
        public uint Param2 { get; set; }

        public Action()
        { }

        public Action(FlatBuffer.Request.Action obj)
        {
            this.Id = obj.Id;
            this.Frame = obj.Frame;
            this.Param1 = obj.Param1;
            this.Param2 = obj.Param2;
        }

        public FlatBuffers.Offset<FlatBuffer.Request.Action> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _id = this.Id;
            var _frame = this.Frame;
            var _param1 = this.Param1;
            var _param2 = this.Param2;

            return FlatBuffer.Request.Action.CreateAction(builder, _id, _frame, _param1, _param2);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static Action Deserialize(byte[] bytes)
        {
            return new Action(FlatBuffer.Request.Action.GetRootAsAction(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class ActionQueue : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.ACTION_QUEUE;

        public List<Action> Actions { get; set; }

        public ActionQueue()
        { }

        public ActionQueue(FlatBuffer.Request.ActionQueue obj)
        {
            this.Actions = Enumerable.Range(0, obj.ActionsLength).Select(x => new Action(obj.Actions(x).Value)).ToList();
        }

        public FlatBuffers.Offset<FlatBuffer.Request.ActionQueue> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _actions = FlatBuffer.Request.ActionQueue.CreateActionsVector(builder, this.Actions.Select(x => x.ToFlatBuffer(builder)).ToArray());

            return FlatBuffer.Request.ActionQueue.CreateActionQueue(builder, _actions);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static ActionQueue Deserialize(byte[] bytes)
        {
            return new ActionQueue(FlatBuffer.Request.ActionQueue.GetRootAsActionQueue(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class GameStart : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.GAME_START;

        

        public GameStart()
        { }

        public GameStart(FlatBuffer.Request.GameStart obj)
        {
            
        }

        public FlatBuffers.Offset<FlatBuffer.Request.GameStart> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            FlatBuffer.Request.GameStart.StartGameStart(builder);
            return FlatBuffer.Request.GameStart.EndGameStart(builder);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static GameStart Deserialize(byte[] bytes)
        {
            return new GameStart(FlatBuffer.Request.GameStart.GetRootAsGameStart(new FlatBuffers.ByteBuffer(bytes)));
        }
    }
}