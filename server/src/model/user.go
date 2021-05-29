package model

import (
	"network"
	"protocol"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type OnReceived func(context actor.Context, receiver *UserActor, protocol protocol.Protocol)
type BindUser struct {
	OnReceived
}

type UserActor struct {
	session *actor.PID

	OnReceived
}

func (state *UserActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *actor.Started:
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

	default:
		context.Send(state.session, msg)
	}
}
