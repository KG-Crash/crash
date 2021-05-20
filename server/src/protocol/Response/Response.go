package response

import (
	"protocol"
	source "protocol/FlatBuffer/Response"

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

	Allocator[KICKED_ROOM] = func(bytes []byte) protocol.Protocol {
		x := &KickedRoom{}
		return x.Deserialize(bytes)
	}

}

const (
	CREATE_ROOM = iota
	JOIN_ROOM
	LEAVE_ROOM
	KICK_ROOM
	KICKED_ROOM
)

type CreateRoom struct {
	Id uint32
}

func (obj *CreateRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.CreateRoomStart(builder)
	source.CreateRoomAddId(builder, obj.Id)

	return source.CreateRoomEnd(builder)
}

func (obj *CreateRoom) parse(x *source.CreateRoom) *CreateRoom {
	obj.Id = x.Id()

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
	Users []uint64
}

func (obj *JoinRoom) users(builder *flatbuffers.Builder, users []uint64) flatbuffers.UOffsetT {
	_size := len(users)
	offsets := make([]uint64, _size)
	for i, x := range users {
		offsets[_size-i-1] = x
	}

	builder.StartVector(4, _size, 4)
	for _, offset := range offsets {
		builder.PrependUint64(offset)
	}
	return builder.EndVector(_size)
}

func (obj *JoinRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_users := obj.users(builder, obj.Users)

	source.JoinRoomStart(builder)
	source.JoinRoomAddUsers(builder, _users)

	return source.JoinRoomEnd(builder)
}

func (obj *JoinRoom) parse(x *source.JoinRoom) *JoinRoom {

	_sizeUsers := x.UsersLength()
	obj.Users = make([]uint64, _sizeUsers)
	for i := 0; i < _sizeUsers; i++ {
		obj.Users = append(obj.Users, x.Users(i))
	}

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
	Success bool
}

func (obj *KickRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.KickRoomStart(builder)
	source.KickRoomAddSuccess(builder, obj.Success)

	return source.KickRoomEnd(builder)
}

func (obj *KickRoom) parse(x *source.KickRoom) *KickRoom {
	obj.Success = x.Success()

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

type KickedRoom struct {
}

func (obj *KickedRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.KickedRoomStart(builder)

	return source.KickedRoomEnd(builder)
}

func (obj *KickedRoom) parse(x *source.KickedRoom) *KickedRoom {

	return obj
}

func (obj *KickedRoom) Identity() int {
	return KICKED_ROOM
}

func (obj *KickedRoom) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *KickedRoom) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsKickedRoom(bytes, 0)
	return obj.parse(root)
}
