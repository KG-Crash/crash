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
        KICKED_ROOM
    }

    public class CreateRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Response.Identity.CREATE_ROOM;

        public uint Id { get; set; }

        public CreateRoom()
        { }

        public CreateRoom(FlatBuffer.Response.CreateRoom obj)
        {
            this.Id = obj.Id;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.CreateRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _id = this.Id;

            return FlatBuffer.Response.CreateRoom.CreateCreateRoom(builder, _id);
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

        public List<ulong> Users { get; set; }

        public JoinRoom()
        { }

        public JoinRoom(FlatBuffer.Response.JoinRoom obj)
        {
            this.Users = Enumerable.Range(0, obj.UsersLength).Select(x => (ulong)System.Convert.ChangeType(x, typeof(ulong))).ToList();
        }

        public FlatBuffers.Offset<FlatBuffer.Response.JoinRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _users = FlatBuffer.Response.JoinRoom.CreateUsersVector(builder, this.Users.ToArray());

            return FlatBuffer.Response.JoinRoom.CreateJoinRoom(builder, _users);
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

        

        public LeaveRoom()
        { }

        public LeaveRoom(FlatBuffer.Response.LeaveRoom obj)
        {
            
        }

        public FlatBuffers.Offset<FlatBuffer.Response.LeaveRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            FlatBuffer.Response.LeaveRoom.StartLeaveRoom(builder);
            return FlatBuffer.Response.LeaveRoom.EndLeaveRoom(builder);
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

        public KickRoom()
        { }

        public KickRoom(FlatBuffer.Response.KickRoom obj)
        {
            this.Success = obj.Success;
        }

        public FlatBuffers.Offset<FlatBuffer.Response.KickRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _success = this.Success;

            return FlatBuffer.Response.KickRoom.CreateKickRoom(builder, _success);
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

        

        public KickedRoom()
        { }

        public KickedRoom(FlatBuffer.Response.KickedRoom obj)
        {
            
        }

        public FlatBuffers.Offset<FlatBuffer.Response.KickedRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            FlatBuffer.Response.KickedRoom.StartKickedRoom(builder);
            return FlatBuffer.Response.KickedRoom.EndKickedRoom(builder);
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
}