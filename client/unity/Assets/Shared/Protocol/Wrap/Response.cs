using System.Collections.Generic;
using System.Linq;

namespace Protocol.Response
{
    public enum Identity
    {
        CREATE_ROOM,
        JOIN_ROOM,
        LEAVE_ROOM,
        KICK_ROOM,
        KICKED_ROOM,
        DESTROYED_ROOM
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

    public class JoinRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.JOIN_ROOM;

        public List<string> Users { get; set; }
        public uint Error { get; set; }

        public JoinRoom()
        { }

        public JoinRoom(FlatBuffer.Response.JoinRoom obj)
        {
            this.Users = Enumerable.Range(0, obj.UsersLength).Select(x => obj.Users(x)).ToList();
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.JoinRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _users = FlatBuffer.Response.JoinRoom.CreateUsersVector(builder, this.Users.Select(x => builder.CreateString(x)).ToArray());
            var _error = this.Error;

            return FlatBuffer.Response.JoinRoom.CreateJoinRoom(builder, _users, _error);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static JoinRoom Deserialize(byte[] bytes)
        {
            return new JoinRoom(FlatBuffer.Response.JoinRoom.GetRootAsJoinRoom(new FlatBuffers.ByteBuffer(bytes)));
        }
    }

    public class LeaveRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.LEAVE_ROOM;

        public string Id { get; set; }
        public uint Error { get; set; }

        public LeaveRoom()
        { }

        public LeaveRoom(FlatBuffer.Response.LeaveRoom obj)
        {
            this.Id = obj.Id;
            this.Error = obj.Error;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.LeaveRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _id = builder.CreateString(this.Id);
            var _error = this.Error;

            return FlatBuffer.Response.LeaveRoom.CreateLeaveRoom(builder, _id, _error);
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
}