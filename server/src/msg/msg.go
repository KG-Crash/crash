package msg

import (
	"exception"
	"protocol/request"

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
	Users  []UserState
	Teams  map[int]*actor.PIDSet
	Master User
}

type RequestEnterRoom struct {
	Sender *actor.PID
}

type ResponseEnterRoom struct {
	PID       *actor.PID
	RoomState RoomConfig
	Users     []UserState
	Teams     map[int]*actor.PIDSet
	Master    User
	Error     exception.Error
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
	NewMaster *User
}

type Chat struct {
	User    string
	Message string
}

type Whisper struct {
	From    string
	To      string
	Message string
}

type RequestKick struct {
	From *actor.PID
	To   *actor.PID
}

type Kicked struct {
	Error string
}

type GameStart struct {
	Sender *actor.PID
}

type RequestGetUsers struct{}

type ResponseGetUsers struct {
	Users  []UserState
	Master User
}

type Ready struct{}

type Resume struct {
	User string
}

type ResponseReady struct {
	Seed       int64
	Users      []UserState
	ReadyState []string
}

type Action struct {
	Sender  *actor.PID
	UID     string
	Actions []request.Action
	Turn    int32
}

type InGameChat struct {
	Sender  *actor.PID
	UID     string
	Message string
	Frame   int32
	Turn    int32
}

/*
	user messages
*/

type User struct {
	ID  string
	PID *actor.PID
}

type UserState struct {
	User
	Team int
}

type RequestGetUserState struct{}

type ResponseGetUserState struct {
	PID  *actor.PID
	User User
}

type Disconnected struct {
	ID string
}
