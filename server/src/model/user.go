package model

import (
	"log"
	"msg"
	"network"
	"protocol"
	"protocol/response"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type UserActor struct {
	session *actor.PID

	Id   string
	Room *actor.PID

	msg.OnReceived
}

func NewUser(id string) *UserActor {
	return &UserActor{
		Id: id,
	}
}

func (state *UserActor) OnStarted(context actor.Context) {
	props := actor.PropsFromProducer(func() actor.Actor {
		return &network.SessionActor{}
	})

	state.session = context.Spawn(props)

	context.Send(context.Self(), &response.Login{
		Id:    state.Id,
		Error: 0,
	})
}

// game > user
func (state *UserActor) OnBindUser(context actor.Context, msg *msg.BindUser) {
	state.Id = msg.Id
	state.OnReceived = msg.OnReceived
}

// room > user
func (state *UserActor) OnJoinedRoom(context actor.Context, msg *msg.JoinedRoom) {
	if msg.Error > 0 {
		context.Send(context.Self(), &response.JoinRoom{
			User:  msg.UserId,
			Error: msg.Error,
		})
		return
	}

	if msg.User == context.Self() {
		if state.Room != nil { // 이미 게임방에 입장해있는 경우
			context.Send(context.Self(), &response.JoinRoom{
				User:  msg.UserId,
				Error: 1,
			})
			return
		}

		state.Room = msg.Room
	}

	context.Send(context.Self(), &response.JoinRoom{
		User:  msg.UserId,
		Users: msg.Users,
		Error: 0,
	})
}

// game > user > room
func (state *UserActor) OnLeaveRoom(context actor.Context, msg *msg.LeaveRoom) {
	if state.Room == nil { // 나가려는데 참여하는 방이 없는상태임
		context.Send(context.Self(), &response.LeaveRoom{Error: 1})
	} else {
		context.Send(state.Room, msg)
	}
}

// room > user
func (state *UserActor) OnLeavedRoom(context actor.Context, msg *msg.LeavedRoom) {
	if msg.Error > 0 {
		context.Send(context.Self(), &response.LeaveRoom{Error: msg.Error})
		return
	}

	if msg.User == context.Self() {
		state.Room = nil
	}
	context.Send(context.Self(), &response.LeaveRoom{
		User:  msg.UserId,
		Error: 0,
	})

	log.Printf("User [%s] received another user [%s] leave from room", state.Id, msg.UserId)
}

// room > user
func (state *UserActor) OnDestroyedRoom(context actor.Context, x *msg.DestroyedRoom) {
	state.Room = nil
	context.Send(context.Self(), &response.DestroyedRoom{Error: 0})

	log.Printf("User [%s]'s room is destroyed", state.Id)
}

// user direct
func (state *UserActor) OnChat(context actor.Context, x *msg.Chat) {
	if state.Room == nil {
		context.Send(context.Self(), &response.Chat{
			Error: 1,
		})
		return
	}

	context.Send(state.Room, x)
	log.Printf("User [%s] : %s", x.UserId, x.Message)
}

// user direct
func (state *UserActor) OnKick(context actor.Context, x *msg.Kick) {
	if state.Room == nil {
		return
	}

	context.Send(state.Room, &msg.Kick{
		From: state.Id,
		To:   x.To,
	})
}

// room > user
func (state *UserActor) OnKicked(context actor.Context, x *msg.Kicked) {
	state.Room = nil
	context.Send(context.Self(), &response.KickedRoom{})

	log.Printf("User [%s] has been kicked", state.Id)
}

func (state *UserActor) Receive(context actor.Context) {
	switch x := context.Message().(type) {
	case *actor.Started:
		state.OnStarted(context)

	case *actor.Terminated:
		context.Send(context.Parent(), &msg.Logout{
			UserId: state.Id,
		})

		if state.Room != nil {
			context.Send(state.Room, &msg.LeaveRoom{
				User:   context.Self(),
				UserId: state.Id,
			})
		}

	case *msg.SetConnection:
		context.Send(state.session, x)

	case *msg.Received:
		if state.OnReceived != nil {
			state.OnReceived(context, state.Id, x.Protocol)
		}

	case *msg.BindUser:
		state.OnBindUser(context, x)

	case *msg.JoinedRoom:
		state.OnJoinedRoom(context, x)

	case *msg.LeaveRoom:
		state.OnLeaveRoom(context, x)

	case *msg.LeavedRoom:
		state.OnLeavedRoom(context, x)

	case *msg.DestroyedRoom:
		state.OnDestroyedRoom(context, x)

	case *msg.Chat:
		state.OnChat(context, x)

	case *msg.Kick:
		state.OnKick(context, x)

	case *msg.Kicked:
		state.OnKicked(context, x)

	case protocol.Protocol:
		context.Send(state.session, &msg.Write{
			Protocol: x,
		})

	default:
		context.Send(context.Parent(), x)
	}
}
