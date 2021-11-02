package msg

import (
	"github.com/AsynkronIT/protoactor-go/actor"
)

/*
	game messages
*/

type RequestGetRoom struct {
	ID string
}

type ResponseGetRoom struct {
	Room *actor.PID
}

type RequestGetRoomList struct{}

type ResponseGetRoomList struct {
	Rooms actor.PIDSet
}

type RequestGetUser struct {
	ID string
}

type ResponseGetUser struct {
	User *actor.PID
}

type RequestCreateRoom RoomConfig

type ResponseCreateRoom struct {
	ID   string
	Room *actor.PID
}

/*
	room messages
*/

type RoomConfig struct {
	ID    string
	Title string
	Teams []int32
}

type DestroyRoom struct {
	ID string
}

type RequestGetRoomState struct{}

type ResponseGetRoomState struct {
	PID    *actor.PID
	State  RoomConfig
	Users  map[int][]UserState
	Teams  map[int]*actor.PIDSet
	Master UserState
}

type RequestEnterRoom struct {
	Sender *actor.PID
}

type ResponseEnterRoom struct {
	PID       *actor.PID
	RoomState RoomConfig
	Users     map[int][]UserState
	Teams     map[int]*actor.PIDSet
	Master    UserState
	Error     int
}

// 퇴장 요청
type Leave struct {
	User *actor.PID
	UID  string
}

// 퇴장 요청한 유저에게 전달되는 메시지
type LeftSelf struct{}

// 특정 유저가 퇴장시 나머지 유저들에게 전달되는 메시지
type Left struct {
	User      *actor.PID
	UID       string
	NewMaster *UserState
}

type Chat struct {
	User    string
	Message string
}

type ReceiveChat struct {
	User    string
	Message string
}

type Whisper struct {
	From    string
	To      string
	Message string
}

type Kick struct {
	From *actor.PID
	To   *actor.PID
}

type Kicked struct {
	Error string
}

/*
	user messages
*/

type UserState struct {
	ID  string
	PID *actor.PID
}

type RequestGetUserState struct{}

type ResponseGetUserState struct {
	PID   *actor.PID
	State UserState
}

type Disconnected struct {
	ID string
}
