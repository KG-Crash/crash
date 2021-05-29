package network

import (
	"fmt"
	"log"
	"net"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type OnAccepted func(context actor.Context, acceptor *AcceptorActor, conn net.Conn)

type AcceptorActor struct {
	net.Listener
	OnAccepted
}

type Listen struct {
	Port uint16
}

type Accept struct {
}

type Accepted struct {
	net.Conn
}

type BindAcceptor struct {
	OnAccepted
}

func (state *AcceptorActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *Listen:
		state.Listener, _ = net.Listen("tcp", fmt.Sprintf(":%d", msg.Port))
		context.ActorSystem().Root.Send(context.Self(), &Accept{})
		log.Printf("CRASH SERVER IS RUNNING : %d", msg.Port)

	case *Accept:
		conn, _ := state.Listener.Accept()

		if state.OnAccepted != nil {
			state.OnAccepted(context, state, conn)
		}
		context.Send(context.Self(), &Accepted{Conn: conn})
		context.Send(context.Self(), &Accept{})

	case *BindAcceptor:
		state.OnAccepted = msg.OnAccepted
	}
}
