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

	case ENTER_ROOM:
		x := &EnterRoom{}
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

	case IN_GAME_CHAT:
		x := &InGameChat{}
		return x.Deserialize(payload)

	case ACTION:
		x := &Action{}
		return x.Deserialize(payload)

	case ACTION_QUEUE:
		x := &ActionQueue{}
		return x.Deserialize(payload)

	case GAME_START:
		x := &GameStart{}
		return x.Deserialize(payload)

	case READY:
		x := &Ready{}
		return x.Deserialize(payload)

	}

	return nil
}

func Text(p protocol.Protocol) string {
	switch p.(type) {
	case *CreateRoom:
		return "CREATE_ROOM"

	case *EnterRoom:
		return "ENTER_ROOM"

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

	case *InGameChat:
		return "IN_GAME_CHAT"

	case *Action:
		return "ACTION"

	case *ActionQueue:
		return "ACTION_QUEUE"

	case *GameStart:
		return "GAME_START"

	case *Ready:
		return "READY"
	}
	return ""
}

const (
	CREATE_ROOM = iota
	ENTER_ROOM
	LEAVE_ROOM
	KICK_ROOM
	ROOM_LIST
	CHAT
	WHISPER
	IN_GAME_CHAT
	ACTION
	ACTION_QUEUE
	GAME_START
	READY
)

type CreateRoom struct {
	Title string
	Teams []int32
}

func (obj *CreateRoom) teams(builder *flatbuffers.Builder, teams []int32) flatbuffers.UOffsetT {
	_size := len(teams)
	offsets := make([]int32, _size)
	for i, x := range teams {
		offsets[_size-i-1] = x
	}

	builder.StartVector(4, _size, 4)
	for _, offset := range offsets {
		builder.PrependInt32(offset)
	}
	return builder.EndVector(_size)
}

func (obj *CreateRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_title := builder.CreateString(obj.Title)
	_teams := obj.teams(builder, obj.Teams)

	source.CreateRoomStart(builder)
	source.CreateRoomAddTitle(builder, _title)
	source.CreateRoomAddTeams(builder, _teams)

	return source.CreateRoomEnd(builder)
}

func (obj *CreateRoom) parse(x *source.CreateRoom) *CreateRoom {
	obj.Title = string(x.Title())

	obj.Teams = []int32{}
	for i := 0; i < x.TeamsLength(); i++ {
		obj.Teams = append(obj.Teams, x.Teams(i))
	}

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

type EnterRoom struct {
	Id string
}

func (obj *EnterRoom) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_id := builder.CreateString(obj.Id)

	source.EnterRoomStart(builder)
	source.EnterRoomAddId(builder, _id)

	return source.EnterRoomEnd(builder)
}

func (obj *EnterRoom) parse(x *source.EnterRoom) *EnterRoom {
	obj.Id = string(x.Id())

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

type InGameChat struct {
	Frame   int32
	Message string
}

func (obj *InGameChat) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	_message := builder.CreateString(obj.Message)

	source.InGameChatStart(builder)
	source.InGameChatAddFrame(builder, obj.Frame)
	source.InGameChatAddMessage(builder, _message)

	return source.InGameChatEnd(builder)
}

func (obj *InGameChat) parse(x *source.InGameChat) *InGameChat {
	obj.Frame = x.Frame()
	obj.Message = string(x.Message())

	return obj
}

func (obj *InGameChat) Identity() int {
	return IN_GAME_CHAT
}

func (obj *InGameChat) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *InGameChat) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsInGameChat(bytes, 0)
	return obj.parse(root)
}

type Action struct {
	Id     int32
	Frame  int32
	Param1 uint32
	Param2 uint32
}

func (obj *Action) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.ActionStart(builder)
	source.ActionAddId(builder, obj.Id)
	source.ActionAddFrame(builder, obj.Frame)
	source.ActionAddParam1(builder, obj.Param1)
	source.ActionAddParam2(builder, obj.Param2)

	return source.ActionEnd(builder)
}

func (obj *Action) parse(x *source.Action) *Action {
	obj.Id = x.Id()
	obj.Frame = x.Frame()
	obj.Param1 = x.Param1()
	obj.Param2 = x.Param2()

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
	Turn    int32
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
	source.ActionQueueAddTurn(builder, obj.Turn)

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
	obj.Turn = x.Turn()

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

type GameStart struct {
}

func (obj *GameStart) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.GameStartStart(builder)

	return source.GameStartEnd(builder)
}

func (obj *GameStart) parse(x *source.GameStart) *GameStart {

	return obj
}

func (obj *GameStart) Identity() int {
	return GAME_START
}

func (obj *GameStart) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *GameStart) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsGameStart(bytes, 0)
	return obj.parse(root)
}

type Ready struct {
}

func (obj *Ready) create(builder *flatbuffers.Builder) flatbuffers.UOffsetT {

	source.ReadyStart(builder)

	return source.ReadyEnd(builder)
}

func (obj *Ready) parse(x *source.Ready) *Ready {

	return obj
}

func (obj *Ready) Identity() int {
	return READY
}

func (obj *Ready) Serialize() []byte {

	builder := flatbuffers.NewBuilder(0)
	builder.Finish(obj.create(builder))
	return builder.FinishedBytes()
}

func (obj *Ready) Deserialize(bytes []byte) protocol.Protocol {
	root := source.GetRootAsReady(bytes, 0)
	return obj.parse(root)
}
