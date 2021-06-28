package response

import (
	"encoding/binary"
	"protocol"
	source "protocol/FlatBuffer/Response"

	flatbuffers "github.com/google/flatbuffers/go"
)

func Deserialize(size uint32, bytes []byte) protocol.Protocol {
	offset := 0
	identity := binary.LittleEndian.Uint32(bytes[:4])
	offset += 4

	payload := bytes[offset : offset+int(size)]
	switch identity {
	case LOGIN:
		x := &Login{}
		return x.Deserialize(payload)

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

	case KICKED_ROOM:
		x := &KickedRoom{}
		return x.Deserialize(payload)

	case DESTROYED_ROOM:
		x := &DestroyedRoom{}
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
	case *Login:
		return "LOGIN"

	case *CreateRoom:
		return "CREATE_ROOM"

	case *JoinRoom:
		return "JOIN_ROOM"

	case *LeaveRoom:
		return "LEAVE_ROOM"

	case *KickRoom:
		return "KICK_ROOM"

	case *KickedRoom:
		return "KICKED_ROOM"

	case *DestroyedRoom:
		return "DESTROYED_ROOM"

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
	LOGIN = iota
	CREATE_ROOM
	JOIN_ROOM
	LEAVE_ROOM
	KICK_ROOM
	KICKED_ROOM
	DESTROYED_ROOM
	ROOM_LIST
	CHAT
	WHISPER
)

type Login struct {
	Id    string
	Error uint32
}

func (obj *Login) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_id := builder.CreateString(obj.Id)

	source.LoginStart(builder)
	source.LoginAddId(builder, _id)
	source.LoginAddError(builder, obj.Error)

	return source.LoginEnd(builder)
}

func (obj *Login) parse(x *source.Login) *Login {
	obj.Id = string(x.Id())
	obj.Error = x.Error()

	return obj
}

func (obj *Login) Identity() int {
	return LOGIN
}

func (obj *Login) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *Login) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsLogin(bytes, 0)
	return obj.parse(root)
}

type CreateRoom struct {
	Id    string
	Error uint32
}

func (obj *CreateRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_id := builder.CreateString(obj.Id)

	source.CreateRoomStart(builder)
	source.CreateRoomAddId(builder, _id)
	source.CreateRoomAddError(builder, obj.Error)

	return source.CreateRoomEnd(builder)
}

func (obj *CreateRoom) parse(x *source.CreateRoom) *CreateRoom {
	obj.Id = string(x.Id())
	obj.Error = x.Error()

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
	User   string
	Users  []string
	Master bool
	Error  uint32
}

func (obj *JoinRoom) users(builder *flatbuffers.Builder, users []string) flatbuffers.UOffsetT {
	_size := len(users)
	offsets := make([]flatbuffers.UOffsetT, _size)
	for i, x := range users {
		offsets[_size-i-1] = builder.CreateString(x)
	}

	builder.StartVector(4, _size, 4)
	for _, offset := range offsets {
		builder.PrependUOffsetT(offset)
	}
	return builder.EndVector(_size)
}

func (obj *JoinRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_user := builder.CreateString(obj.User)
	_users := obj.users(builder, obj.Users)

	source.JoinRoomStart(builder)
	source.JoinRoomAddUser(builder, _user)
	source.JoinRoomAddUsers(builder, _users)
	source.JoinRoomAddMaster(builder, obj.Master)
	source.JoinRoomAddError(builder, obj.Error)

	return source.JoinRoomEnd(builder)
}

func (obj *JoinRoom) parse(x *source.JoinRoom) *JoinRoom {
	obj.User = string(x.User())

	_sizeUsers := x.UsersLength()
	obj.Users = make([]string, _sizeUsers)
	for i := 0; i < _sizeUsers; i++ {
		obj.Users = append(obj.Users, string(x.Users(i)))
	}
	obj.Master = x.Master()
	obj.Error = x.Error()

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
	User  string
	Error uint32
}

func (obj *LeaveRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_user := builder.CreateString(obj.User)

	source.LeaveRoomStart(builder)
	source.LeaveRoomAddUser(builder, _user)
	source.LeaveRoomAddError(builder, obj.Error)

	return source.LeaveRoomEnd(builder)
}

func (obj *LeaveRoom) parse(x *source.LeaveRoom) *LeaveRoom {
	obj.User = string(x.User())
	obj.Error = x.Error()

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
	Error   uint32
}

func (obj *KickRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.KickRoomStart(builder)
	source.KickRoomAddSuccess(builder, obj.Success)
	source.KickRoomAddError(builder, obj.Error)

	return source.KickRoomEnd(builder)
}

func (obj *KickRoom) parse(x *source.KickRoom) *KickRoom {
	obj.Success = x.Success()
	obj.Error = x.Error()

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
	Error uint32
}

func (obj *KickedRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.KickedRoomStart(builder)
	source.KickedRoomAddError(builder, obj.Error)

	return source.KickedRoomEnd(builder)
}

func (obj *KickedRoom) parse(x *source.KickedRoom) *KickedRoom {
	obj.Error = x.Error()

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

type DestroyedRoom struct {
	Error uint32
}

func (obj *DestroyedRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.DestroyedRoomStart(builder)
	source.DestroyedRoomAddError(builder, obj.Error)

	return source.DestroyedRoomEnd(builder)
}

func (obj *DestroyedRoom) parse(x *source.DestroyedRoom) *DestroyedRoom {
	obj.Error = x.Error()

	return obj
}

func (obj *DestroyedRoom) Identity() int {
	return DESTROYED_ROOM
}

func (obj *DestroyedRoom) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *DestroyedRoom) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsDestroyedRoom(bytes, 0)
	return obj.parse(root)
}

type RoomList struct {
	Rooms []string
	Error uint32
}

func (obj *RoomList) rooms(builder *flatbuffers.Builder, rooms []string) flatbuffers.UOffsetT {
	_size := len(rooms)
	offsets := make([]flatbuffers.UOffsetT, _size)
	for i, x := range rooms {
		offsets[_size-i-1] = builder.CreateString(x)
	}

	builder.StartVector(4, _size, 4)
	for _, offset := range offsets {
		builder.PrependUOffsetT(offset)
	}
	return builder.EndVector(_size)
}

func (obj *RoomList) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_rooms := obj.rooms(builder, obj.Rooms)

	source.RoomListStart(builder)
	source.RoomListAddRooms(builder, _rooms)
	source.RoomListAddError(builder, obj.Error)

	return source.RoomListEnd(builder)
}

func (obj *RoomList) parse(x *source.RoomList) *RoomList {

	_sizeRooms := x.RoomsLength()
	obj.Rooms = make([]string, _sizeRooms)
	for i := 0; i < _sizeRooms; i++ {
		obj.Rooms = append(obj.Rooms, string(x.Rooms(i)))
	}
	obj.Error = x.Error()

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
	User    string
	Message string
	Error   uint32
}

func (obj *Chat) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_user := builder.CreateString(obj.User)
	_message := builder.CreateString(obj.Message)

	source.ChatStart(builder)
	source.ChatAddUser(builder, _user)
	source.ChatAddMessage(builder, _message)
	source.ChatAddError(builder, obj.Error)

	return source.ChatEnd(builder)
}

func (obj *Chat) parse(x *source.Chat) *Chat {
	obj.User = string(x.User())
	obj.Message = string(x.Message())
	obj.Error = x.Error()

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
	Error   uint32
}

func (obj *Whisper) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_user := builder.CreateString(obj.User)
	_message := builder.CreateString(obj.Message)

	source.WhisperStart(builder)
	source.WhisperAddUser(builder, _user)
	source.WhisperAddMessage(builder, _message)
	source.WhisperAddError(builder, obj.Error)

	return source.WhisperEnd(builder)
}

func (obj *Whisper) parse(x *source.Whisper) *Whisper {
	obj.User = string(x.User())
	obj.Message = string(x.Message())
	obj.Error = x.Error()

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