package request

import (
	"encoding/binary"
	"protocol"
	source "protocol/FlatBuffer/Request"

	flatbuffers "github.com/google/flatbuffers/go"
)

func Deserialize(size uint32, bytes []byte) protocol.Protocol {
	offset := 0
	identity := binary.LittleEndian.Uint32(bytes[:4])
	offset += 4

	payload := bytes[offset : offset+int(size)]
	switch identity {
	case CREATE_ROOM:
		x := &CreateRoom{}
		return x.Deserialize(payload)

	case JOIN_ROOM:
		x := &JoinRoom{}
		return x.Deserialize(payload)

	case LEAVE_ROOM:
		x := &LeaveRoom{}
		return x.Deserialize(payload)

	case KICK_ROOM:
		x := &KickRoom{}
		return x.Deserialize(payload)

	case ROOM_LIST:
		x := &RoomList{}
		return x.Deserialize(payload)

	case CHAT:
		x := &Chat{}
		return x.Deserialize(payload)

	case WHISPER:
		x := &Whisper{}
		return x.Deserialize(payload)

	}

	return nil
}

func Text(p protocol.Protocol) string {
	switch p.(type) {
	case *CreateRoom:
		return "CREATE_ROOM"

	case *JoinRoom:
		return "JOIN_ROOM"

	case *LeaveRoom:
		return "LEAVE_ROOM"

	case *KickRoom:
		return "KICK_ROOM"

	case *RoomList:
		return "ROOM_LIST"

	case *Chat:
		return "CHAT"

	case *Whisper:
		return "WHISPER"
	}
	return ""
}

const (
	CREATE_ROOM = iota
	JOIN_ROOM
	LEAVE_ROOM
	KICK_ROOM
	ROOM_LIST
	CHAT
	WHISPER
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
	Id string
}

func (obj *JoinRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_id := builder.CreateString(obj.Id)

	source.JoinRoomStart(builder)
	source.JoinRoomAddId(builder, _id)

	return source.JoinRoomEnd(builder)
}

func (obj *JoinRoom) parse(x *source.JoinRoom) *JoinRoom {
	obj.Id = string(x.Id())

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
	User string
}

func (obj *KickRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_user := builder.CreateString(obj.User)

	source.KickRoomStart(builder)
	source.KickRoomAddUser(builder, _user)

	return source.KickRoomEnd(builder)
}

func (obj *KickRoom) parse(x *source.KickRoom) *KickRoom {
	obj.User = string(x.User())

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

type RoomList struct {
}

func (obj *RoomList) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.RoomListStart(builder)

	return source.RoomListEnd(builder)
}

func (obj *RoomList) parse(x *source.RoomList) *RoomList {

	return obj
}

func (obj *RoomList) Identity() int {
	return ROOM_LIST
}

func (obj *RoomList) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *RoomList) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsRoomList(bytes, 0)
	return obj.parse(root)
}

type Chat struct {
	Message string
}

func (obj *Chat) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_message := builder.CreateString(obj.Message)

	source.ChatStart(builder)
	source.ChatAddMessage(builder, _message)

	return source.ChatEnd(builder)
}

func (obj *Chat) parse(x *source.Chat) *Chat {
	obj.Message = string(x.Message())

	return obj
}

func (obj *Chat) Identity() int {
	return CHAT
}

func (obj *Chat) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *Chat) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsChat(bytes, 0)
	return obj.parse(root)
}

type Whisper struct {
	User    string
	Message string
}

func (obj *Whisper) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_user := builder.CreateString(obj.User)
	_message := builder.CreateString(obj.Message)

	source.WhisperStart(builder)
	source.WhisperAddUser(builder, _user)
	source.WhisperAddMessage(builder, _message)

	return source.WhisperEnd(builder)
}

func (obj *Whisper) parse(x *source.Whisper) *Whisper {
	obj.User = string(x.User())
	obj.Message = string(x.Message())

	return obj
}

func (obj *Whisper) Identity() int {
	return WHISPER
}

func (obj *Whisper) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *Whisper) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsWhisper(bytes, 0)
	return obj.parse(root)
}
