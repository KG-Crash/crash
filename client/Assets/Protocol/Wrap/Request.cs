using System.Collections.Generic;
using System.Linq;

namespace Protocol.Request
{
    public enum Identity
    {
        CREATE_ROOM,
        JOIN_ROOM,
        LEAVE_ROOM,
        KICK_ROOM
    }

    public class CreateRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.CREATE_ROOM;

        

        public CreateRoom()
        { }

        public CreateRoom(FlatBuffer.Request.CreateRoom obj)
        {
            
        }

        public FlatBuffers.Offset<FlatBuffer.Request.CreateRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            FlatBuffer.Request.CreateRoom.StartCreateRoom(builder);
            return FlatBuffer.Request.CreateRoom.EndCreateRoom(builder);
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

    public class JoinRoom : IProtocol
    {
        public uint Identity => (uint)Protocol.Request.Identity.JOIN_ROOM;

        public uint Id { get; set; }

        public JoinRoom()
        { }

        public JoinRoom(FlatBuffer.Request.JoinRoom obj)
        {
            this.Id = obj.Id;
        }

        public FlatBuffers.Offset<FlatBuffer.Request.JoinRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _id = this.Id;

            return FlatBuffer.Request.JoinRoom.CreateJoinRoom(builder, _id);
        }

        public byte[] Serialize()
        {
            var builder = new FlatBuffers.FlatBufferBuilder(512);
            builder.Finish(this.ToFlatBuffer(builder).Value);
            return builder.DataBuffer.ToSizedArray();
        }

        public static JoinRoom Deserialize(byte[] bytes)
        {
            return new JoinRoom(FlatBuffer.Request.JoinRoom.GetRootAsJoinRoom(new FlatBuffers.ByteBuffer(bytes)));
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

        public ulong User { get; set; }

        public KickRoom()
        { }

        public KickRoom(FlatBuffer.Request.KickRoom obj)
        {
            this.User = obj.User;
        }

        public FlatBuffers.Offset<FlatBuffer.Request.KickRoom> ToFlatBuffer(FlatBuffers.FlatBufferBuilder builder)
        {
            var _user = this.User;

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
}