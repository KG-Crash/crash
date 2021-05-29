package network

import (
	"net"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type SessionActor struct {
	net.Conn

	Sender   *actor.PID
	Receiver *actor.PID
}

type SetConn struct {
	net.Conn
}

type Receive struct {
}

func (state *SessionActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *actor.Started:
		state.Sender = context.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return NewSenderActor()
		}))

		state.Receiver = context.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return NewReceiverActor()
		}))

	case *actor.Stop:
		context.Stop(state.Sender)
		context.Stop(state.Receiver)
		state.Conn.Close()

	case *SetConn:
		state.Conn = msg.Conn
		context.Send(state.Sender, msg)
		context.Send(state.Receiver, msg)

	case *Received:
		context.Send(context.Parent(), msg)

	case *Write:
		context.Send(state.Sender, msg)
	}
}
