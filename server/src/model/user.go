package model

import (
	"log"
	"network"
	"protocol"
	"protocol/response"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type OnReceived func(context actor.Context, receiver *UserActor, protocol protocol.Protocol)

type UserActor struct {
	session *actor.PID

	Id   string
	Room *actor.PID

	OnReceived
}

type BindUser struct {
	Id string
	OnReceived
}

type JoinedRoom struct {
	UserId string
	User   *actor.PID
	Room   *actor.PID
	Users  []string
	Error  uint32
}

type LeaveRoom struct {
	UserId string
	User   *actor.PID
}

type Logout struct {
	UserId string
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
}

// game > user
func (state *UserActor) OnBindUser(context actor.Context, msg *BindUser) {
	state.Id = msg.Id
	state.OnReceived = msg.OnReceived
}

// room > user
func (state *UserActor) OnJoinedRoom(context actor.Context, msg *JoinedRoom) {
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
func (state *UserActor) OnLeaveRoom(context actor.Context, msg *LeaveRoom) {
	if state.Room == nil { // 나가려는데 참여하는 방이 없는상태임
		context.Send(context.Self(), &response.LeaveRoom{Error: 1})
	} else {
		context.Send(state.Room, msg)
	}
}

// room > user
func (state *UserActor) OnLeavedRoom(context actor.Context, msg *LeavedRoom) {
	if msg.Error > 0 {
		context.Send(context.Self(), &response.LeaveRoom{Error: msg.Error})
		return
	}

	if msg.User == context.Self() {
		state.Room = nil
	}
	context.Send(context.Self(), &response.LeaveRoom{
		Id:    msg.UserId,
		Error: 0,
	})

	log.Printf("User [%s] received another user [%s] leave from room", state.Id, msg.UserId)
}

// room > user
func (state *UserActor) OnDestroyedRoom(context actor.Context, msg *DestroyedRoom) {
	state.Room = nil
	context.Send(context.Self(), &response.DestroyedRoom{Error: 0})

	log.Printf("User [%s]'s room is destroyed", state.Id)
}

func (state *UserActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *actor.Started:
		state.OnStarted(context)

	case *actor.Terminated:
		context.Send(context.Parent(), &Logout{
			UserId: state.Id,
		})

		if state.Room != nil {
			context.Send(state.Room, &LeaveRoom{
				User:   context.Self(),
				UserId: state.Id,
			})
		}

	case *network.SetConnection:
		context.Send(state.session, msg)

	case *network.Received:
		if state.OnReceived != nil {
			state.OnReceived(context, state, msg.Protocol)
		}

	case *BindUser:
		state.OnBindUser(context, msg)

	case *JoinedRoom:
		state.OnJoinedRoom(context, msg)

	case *LeaveRoom:
		state.OnLeaveRoom(context, msg)

	case *LeavedRoom:
		state.OnLeavedRoom(context, msg)

	case *DestroyedRoom:
		state.OnDestroyedRoom(context, msg)

	case protocol.Protocol:
		context.Send(state.session, &network.Write{
			Protocol: msg,
		})

	default:
		context.Send(context.Parent(), msg)
	}
}
