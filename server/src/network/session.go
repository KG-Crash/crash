package network

import (
	"net"
	"protocol"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type OnReceived func(context actor.Context, receiver *SessionActor, protocol protocol.Protocol)

type SessionActor struct {
	net.Conn

	OnReceived

	Sender   *actor.PID
	Receiver *actor.PID
}

type SetConn struct {
	net.Conn
}

type Receive struct {
}

type BindSession struct {
	OnReceived
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

	case *BindSession:
		state.OnReceived = msg.OnReceived

	case *SetConn:
		state.Conn = msg.Conn
		context.Send(state.Sender, msg)
		context.Send(state.Receiver, msg)

	case *Received:
		if state.OnReceived != nil {
			state.OnReceived(context, state, msg.Protocol)
		}

	case *Write:
		context.Send(state.Sender, msg)
	}
}
