package session

import (
	"net"

	"github.com/AsynkronIT/protoactor-go/actor"
	"kg.crash.com/session/receiver"
	"kg.crash.com/session/sender"
)

type Actor struct {
	net.Conn

	Sender   *actor.PID
	Receiver *actor.PID
}

func New(conn net.Conn) *Actor {
	return &Actor{
		Conn: conn,
	}
}

func (state *Actor) Receive(ctx actor.Context) {
	switch x := ctx.Message().(type) {
	case *actor.Started:
		state.Sender = ctx.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return sender.New(state.Conn)
		}))

		state.Receiver = ctx.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return receiver.New(state.Conn)
		}))

	case *actor.Terminated:
		state.Conn.Close()

	case *receiver.Disconnected:
		ctx.Send(ctx.Parent(), x)

	case *receiver.Received:
		ctx.Send(ctx.Parent(), x)

	case *sender.Send:
		ctx.Send(state.Sender, x)
	}
}
