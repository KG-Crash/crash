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

	case USER:
		x := &User{}
		return x.Deserialize(payload)

	case ENTER_ROOM:
		x := &EnterRoom{}
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

	case ACTION:
		x := &Action{}
		return x.Deserialize(payload)

	case ACTION_QUEUE:
		x := &ActionQueue{}
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

	case *User:
		return "USER"

	case *EnterRoom:
		return "ENTER_ROOM"

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

	case *Action:
		return "ACTION"

	case *ActionQueue:
		return "ACTION_QUEUE"
	}
	return ""
}

const (
	LOGIN = iota
	CREATE_ROOM
	USER
	ENTER_ROOM
	LEAVE_ROOM
	KICK_ROOM
	KICKED_ROOM
	DESTROYED_ROOM
	ROOM_LIST
	CHAT
	WHISPER
	ACTION
	ACTION_QUEUE
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

type User struct {
	Id   string
	Team int32
}

func (obj *User) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_id := builder.CreateString(obj.Id)

	source.UserStart(builder)
	source.UserAddId(builder, _id)
	source.UserAddTeam(builder, obj.Team)

	return source.UserEnd(builder)
}

func (obj *User) parse(x *source.User) *User {
	obj.Id = string(x.Id())
	obj.Team = x.Team()

	return obj
}

func (obj *User) Identity() int {
	return USER
}

func (obj *User) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *User) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsUser(bytes, 0)
	return obj.parse(root)
}

type EnterRoom struct {
	User   string
	Users  []User
	Master string
	Error  uint32
}

func (obj *EnterRoom) users(builder *flatbuffers.Builder, users []User) flatbuffers.UOffsetT {
	_size := len(users)
	offsets := make([]flatbuffers.UOffsetT, _size)
	for i, x := range users {
		offsets[_size-i-1] = x.create(builder)
	}

	builder.StartVector(4, _size, 4)
	for _, offset := range offsets {
		builder.PrependUOffsetT(offset)
	}
	return builder.EndVector(_size)
}

func (obj *EnterRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_user := builder.CreateString(obj.User)
	_users := obj.users(builder, obj.Users)
	_master := builder.CreateString(obj.Master)

	source.EnterRoomStart(builder)
	source.EnterRoomAddUser(builder, _user)
	source.EnterRoomAddUsers(builder, _users)
	source.EnterRoomAddMaster(builder, _master)
	source.EnterRoomAddError(builder, obj.Error)

	return source.EnterRoomEnd(builder)
}

func (obj *EnterRoom) parse(x *source.EnterRoom) *EnterRoom {
	obj.User = string(x.User())

	obj.Users = []User{}
	for i := 0; i < x.UsersLength(); i++ {
		_user := &source.User{}
		x.Users(_user, i)

		user := User{}
		user.parse(_user)
		obj.Users = append(obj.Users, user)
	}
	obj.Master = string(x.Master())
	obj.Error = x.Error()

	return obj
}

func (obj *EnterRoom) Identity() int {
	return ENTER_ROOM
}

func (obj *EnterRoom) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *EnterRoom) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsEnterRoom(bytes, 0)
	return obj.parse(root)
}

type LeaveRoom struct {
	User      string
	NewMaster string
	Error     uint32
}

func (obj *LeaveRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_user := builder.CreateString(obj.User)
	_newMaster := builder.CreateString(obj.NewMaster)

	source.LeaveRoomStart(builder)
	source.LeaveRoomAddUser(builder, _user)
	source.LeaveRoomAddNewMaster(builder, _newMaster)
	source.LeaveRoomAddError(builder, obj.Error)

	return source.LeaveRoomEnd(builder)
}

func (obj *LeaveRoom) parse(x *source.LeaveRoom) *LeaveRoom {
	obj.User = string(x.User())
	obj.NewMaster = string(x.NewMaster())
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

	obj.Rooms = []string{}
	for i := 0; i < x.RoomsLength(); i++ {
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
	From    string
	To      string
	Message string
	Error   uint32
}

func (obj *Whisper) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_from := builder.CreateString(obj.From)
	_to := builder.CreateString(obj.To)
	_message := builder.CreateString(obj.Message)

	source.WhisperStart(builder)
	source.WhisperAddFrom(builder, _from)
	source.WhisperAddTo(builder, _to)
	source.WhisperAddMessage(builder, _message)
	source.WhisperAddError(builder, obj.Error)

	return source.WhisperEnd(builder)
}

func (obj *Whisper) parse(x *source.Whisper) *Whisper {
	obj.From = string(x.From())
	obj.To = string(x.To())
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

type Action struct {
	User      string
	Frame     int32
	Id        int32
	PositionX int32
	PositionY int32
}

func (obj *Action) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_user := builder.CreateString(obj.User)

	source.ActionStart(builder)
	source.ActionAddUser(builder, _user)
	source.ActionAddFrame(builder, obj.Frame)
	source.ActionAddId(builder, obj.Id)
	source.ActionAddPositionX(builder, obj.PositionX)
	source.ActionAddPositionY(builder, obj.PositionY)

	return source.ActionEnd(builder)
}

func (obj *Action) parse(x *source.Action) *Action {
	obj.User = string(x.User())
	obj.Frame = x.Frame()
	obj.Id = x.Id()
	obj.PositionX = x.PositionX()
	obj.PositionY = x.PositionY()

	return obj
}

func (obj *Action) Identity() int {
	return ACTION
}

func (obj *Action) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *Action) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsAction(bytes, 0)
	return obj.parse(root)
}

type ActionQueue struct {
	Actions []Action
}

func (obj *ActionQueue) actions(builder *flatbuffers.Builder, actions []Action) flatbuffers.UOffsetT {
	_size := len(actions)
	offsets := make([]flatbuffers.UOffsetT, _size)
	for i, x := range actions {
		offsets[_size-i-1] = x.create(builder)
	}

	builder.StartVector(4, _size, 4)
	for _, offset := range offsets {
		builder.PrependUOffsetT(offset)
	}
	return builder.EndVector(_size)
}

func (obj *ActionQueue) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_actions := obj.actions(builder, obj.Actions)

	source.ActionQueueStart(builder)
	source.ActionQueueAddActions(builder, _actions)

	return source.ActionQueueEnd(builder)
}

func (obj *ActionQueue) parse(x *source.ActionQueue) *ActionQueue {

	obj.Actions = []Action{}
	for i := 0; i < x.ActionsLength(); i++ {
		_action := &source.Action{}
		x.Actions(_action, i)

		action := Action{}
		action.parse(_action)
		obj.Actions = append(obj.Actions, action)
	}

	return obj
}

func (obj *ActionQueue) Identity() int {
	return ACTION_QUEUE
}

func (obj *ActionQueue) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *ActionQueue) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsActionQueue(bytes, 0)
	return obj.parse(root)
}
