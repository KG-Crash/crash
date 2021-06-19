package model

import (
	"log"
	"msg"
	"protocol/response"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type RoomActor struct {
	Id     string
	Users  map[string]*actor.PID
	Master *actor.PID
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
func (state *RoomActor) OnJoinRoom(context actor.Context, x *msg.JoinRoom) {
	if _, ok := state.Users[x.UserId]; ok { // 이미 참여중인 유저가 다시 참여하려고 함
		context.Send(x.User, &msg.JoinedRoom{Error: 1})
		return
	}

	state.Users[x.UserId] = x.User

	for _, user := range state.Users {
		context.Send(user, &msg.JoinedRoom{
			UserId: x.UserId,
			User:   x.User,
			Master: state.Master == x.User,
			Room:   context.Self(),
			Users:  state.UserIdList(),
			Error:  0,
		})
	}

	log.Printf("User [%s] joined into room [%s]", x.UserId, state.Id)
}

// game > user > room
func (state *RoomActor) OnLeaveRoom(context actor.Context, x *msg.LeaveRoom) {

	if _, ok := state.Users[x.UserId]; !ok { // Room 소속 유저가 아님
		context.Send(x.User, &msg.LeavedRoom{
			Error: 1,
		})
		return
	}

	if x.User == state.Master {
		delete(state.Users, x.UserId)

		context.Send(x.User, &msg.LeavedRoom{
			User:   x.User,
			UserId: x.UserId,
		})

		response := &msg.DestroyedRoom{RoomId: state.Id}
		context.Send(context.Parent(), response)
		for _, user := range state.Users {
			context.Send(user, response)
		}

		context.Stop(context.Self())

	} else {
		response := &msg.LeavedRoom{
			User:   x.User,
			UserId: x.UserId,
			Error:  0,
		}
		for _, user := range state.Users {
			context.Send(user, response)
		}

		delete(state.Users, x.UserId)
	}
}

func (state *RoomActor) OnChat(context actor.Context, x *msg.Chat) {
	if _, ok := state.Users[x.UserId]; !ok {
		return
	}

	for _, user := range state.Users {
		context.Send(user, &response.Chat{
			User:    x.UserId,
			Message: x.Message,
			Error:   0,
		})
	}
}

func (state *RoomActor) OnKick(context actor.Context, x *msg.Kick) {
	if _, ok := state.Users[x.From]; !ok {
		return
	}

	if _, ok := state.Users[x.To]; !ok {
		return
	}

	if state.Users[x.From] != state.Master {
		return
	}

	if state.Users[x.To] == state.Master {
		return
	}

	to := state.Users[x.To]
	context.Send(to, &msg.Kicked{})

	delete(state.Users, x.To)
	for _, user := range state.Users {
		context.Send(user, &response.LeaveRoom{
			User: x.To,
		})
	}
}

func (state *RoomActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *msg.JoinRoom:
		state.OnJoinRoom(context, msg)

	case *msg.LeaveRoom:
		state.OnLeaveRoom(context, msg)

	case *msg.Chat:
		state.OnChat(context, msg)

	case *msg.Kick:
		state.OnKick(context, msg)
	}
}
