package model

import (
	"log"
	"protocol/response"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type RoomActor struct {
	Id     string
	Users  map[string]*actor.PID
	Master *actor.PID
}

type JoinRoom struct {
	User   *actor.PID
	UserId string
	RoomId string
}

type LeavedRoom struct {
	User   *actor.PID
	UserId string
	Error  uint32
}

type DestroyedRoom struct {
	RoomId string
}

func NewRoom(id string, master *actor.PID) *RoomActor {
	return &RoomActor{
		Id:     id,
		Users:  make(map[string]*actor.PID),
		Master: master,
	}
}

func (state *RoomActor) UserIdList() []string {
	result := make([]string, 0, len(state.Users))
	for id, _ := range state.Users {
		result = append(result, id)
	}

	return result
}

// game > room
func (state *RoomActor) OnJoinRoom(context actor.Context, msg *JoinRoom) {
	if _, ok := state.Users[msg.UserId]; ok { // 이미 참여중인 유저가 다시 참여하려고 함
		context.Send(msg.User, &JoinedRoom{Error: 1})
		return
	}

	state.Users[msg.UserId] = msg.User

	for _, user := range state.Users {
		context.Send(user, &JoinedRoom{
			UserId: msg.UserId,
			User:   msg.User,
			Room:   context.Self(),
			Users:  state.UserIdList(),
			Error:  0,
		})
	}

	log.Printf("User [%s] joined into room [%s]", msg.UserId, state.Id)
}

// game > user > room
func (state *RoomActor) OnLeaveRoom(context actor.Context, msg *LeaveRoom) {

	if _, ok := state.Users[msg.UserId]; !ok { // Room 소속 유저가 아님
		context.Send(msg.User, &LeavedRoom{
			Error: 1,
		})
		return
	}

	if msg.User == state.Master {
		delete(state.Users, msg.UserId)

		context.Send(msg.User, &LeavedRoom{
			User:   msg.User,
			UserId: msg.UserId,
		})

		response := &DestroyedRoom{RoomId: state.Id}
		context.Send(context.Parent(), response)
		for _, user := range state.Users {
			context.Send(user, response)
		}

		context.Stop(context.Self())

	} else {
		response := &LeavedRoom{
			User:   msg.User,
			UserId: msg.UserId,
			Error:  0,
		}
		for _, user := range state.Users {
			context.Send(user, response)
		}

		delete(state.Users, msg.UserId)
	}
}

func (state *RoomActor) OnChat(context actor.Context, msg *Chat) {
	if _, ok := state.Users[msg.UserId]; !ok {
		return
	}

	for _, user := range state.Users {
		context.Send(user, &response.Chat{
			User:    msg.UserId,
			Message: msg.Message,
			Error:   0,
		})
	}
}

func (state *RoomActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *JoinRoom:
		state.OnJoinRoom(context, msg)

	case *LeaveRoom:
		state.OnLeaveRoom(context, msg)

	case *Chat:
		state.OnChat(context, msg)
	}
}
