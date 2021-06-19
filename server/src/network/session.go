package network

import (
	"msg"
	"net"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type SessionActor struct {
	net.Conn

	Sender   *actor.PID
	Receiver *actor.PID
}

func (state *SessionActor) Receive(context actor.Context) {
	switch x := context.Message().(type) {
	case *actor.Started:
		state.Sender = context.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return NewSenderActor()
		}))

		state.Receiver = context.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return NewReceiverActor()
		}))

	case *actor.Terminated:
		state.Conn.Close()
		context.Stop(context.Parent())

	case *actor.Stop:
		context.Stop(state.Sender)
		context.Stop(state.Receiver)
		state.Conn.Close()

	case *msg.SetConnection:
		state.Conn = x.Conn
		context.Send(state.Sender, x)
		context.Send(state.Receiver, x)

	case *msg.Received:
		context.Send(context.Parent(), x)

	case *msg.Write:
		context.Send(state.Sender, x)
	}
}
