package request

import (
	"protocol"
	source "protocol/FlatBuffer/Request"

	flatbuffers "github.com/google/flatbuffers/go"
)

var Allocator map[uint32]func([]byte) protocol.Protocol

func init() {
	Allocator = make(map[uint32]func([]byte) protocol.Protocol)

	Allocator[CREATE_ROOM] = func(bytes []byte) protocol.Protocol {
		x := &CreateRoom{}
		return x.Deserialize(bytes)
	}

	Allocator[JOIN_ROOM] = func(bytes []byte) protocol.Protocol {
		x := &JoinRoom{}
		return x.Deserialize(bytes)
	}

	Allocator[LEAVE_ROOM] = func(bytes []byte) protocol.Protocol {
		x := &LeaveRoom{}
		return x.Deserialize(bytes)
	}

	Allocator[KICK_ROOM] = func(bytes []byte) protocol.Protocol {
		x := &KickRoom{}
		return x.Deserialize(bytes)
	}

}

const (
	CREATE_ROOM = iota
	JOIN_ROOM
	LEAVE_ROOM
	KICK_ROOM
)

type CreateRoom struct {
}

func (obj *CreateRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.CreateRoomStart(builder)

	return source.CreateRoomEnd(builder)
}

func (obj *CreateRoom) parse(x *source.CreateRoom) *CreateRoom {

	return obj
}

func (obj *CreateRoom) Identity() int {
	return CREATE_ROOM
}

func (obj *CreateRoom) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *CreateRoom) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsCreateRoom(bytes, 0)
	return obj.parse(root)
}

type JoinRoom struct {
	Id uint32
}

func (obj *JoinRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.JoinRoomStart(builder)
	source.JoinRoomAddId(builder, obj.Id)

	return source.JoinRoomEnd(builder)
}

func (obj *JoinRoom) parse(x *source.JoinRoom) *JoinRoom {
	obj.Id = x.Id()

	return obj
}

func (obj *JoinRoom) Identity() int {
	return JOIN_ROOM
}

func (obj *JoinRoom) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *JoinRoom) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsJoinRoom(bytes, 0)
	return obj.parse(root)
}

type LeaveRoom struct {
}

func (obj *LeaveRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.LeaveRoomStart(builder)

	return source.LeaveRoomEnd(builder)
}

func (obj *LeaveRoom) parse(x *source.LeaveRoom) *LeaveRoom {

	return obj
}

func (obj *LeaveRoom) Identity() int {
	return LEAVE_ROOM
}

func (obj *LeaveRoom) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *LeaveRoom) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsLeaveRoom(bytes, 0)
	return obj.parse(root)
}

type KickRoom struct {
	User uint64
}

func (obj *KickRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.KickRoomStart(builder)
	source.KickRoomAddUser(builder, obj.User)

	return source.KickRoomEnd(builder)
}

func (obj *KickRoom) parse(x *source.KickRoom) *KickRoom {
	obj.User = x.User()

	return obj
}

func (obj *KickRoom) Identity() int {
	return KICK_ROOM
}

func (obj *KickRoom) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *KickRoom) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsKickRoom(bytes, 0)
	return obj.parse(root)
}
