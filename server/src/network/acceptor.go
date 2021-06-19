package network

import (
	"fmt"
	"log"
	"msg"
	"net"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type AcceptorActor struct {
	net.Listener
	msg.OnAccepted
}

func (state *AcceptorActor) Receive(context actor.Context) {
	switch x := context.Message().(type) {
	case *msg.Listen:
		state.Listener, _ = net.Listen("tcp", fmt.Sprintf(":%d", x.Port))
		context.ActorSystem().Root.Send(context.Self(), &msg.Accept{})
		log.Printf("CRASH SERVER IS RUNNING : %d", x.Port)

	case *msg.Accept:
		conn, _ := state.Listener.Accept()

		if state.OnAccepted != nil {
			state.OnAccepted(context, conn)
		}
		context.Send(context.Self(), &msg.Accepted{Conn: conn})
		context.Send(context.Self(), &msg.Accept{})

	case *msg.BindAcceptor:
		state.OnAccepted = x.OnAccepted
	}
}
