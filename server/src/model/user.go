package model

import (
	"network"
	"protocol"

	"github.com/AsynkronIT/protoactor-go/actor"
	"github.com/google/uuid"
)

type OnReceived func(context actor.Context, receiver *UserActor, protocol protocol.Protocol)

type UserActor struct {
	session *actor.PID

	Id   string
	Room *actor.PID

	OnReceived
}

type BindUser struct {
	OnReceived
}

type JoinRoom struct {
	Room *actor.PID
}

type LeaveRoom struct {
}

func (state *UserActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *actor.Started:
		state.Id = uuid.NewString()
		props := actor.PropsFromProducer(func() actor.Actor {
			return &network.SessionActor{}
		})

		state.session = context.Spawn(props)

	case *BindUser:
		state.OnReceived = msg.OnReceived

	case *network.Received:
		if state.OnReceived != nil {
			state.OnReceived(context, state, msg.Protocol)
		}

	case *JoinRoom:
		if state.Room != nil {
			context.Send(context.Parent(), Exception{What: 1}) // TODO Exception ID 부여
		} else {
			state.Room = msg.Room
		}

	case *LeaveRoom:
		if state.Room == nil {
			context.Send(context.Parent(), Exception{What: 2})
		} else {
			state.Room = nil
		}

	default:
		context.Send(state.session, msg)
	}
}
