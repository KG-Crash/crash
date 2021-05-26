package network

import (
	"fmt"
	"log"
	"net"
	"protocol"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type AcceptorActor struct {
	net.Listener
}

type Listen struct {
	Port uint16
}

type Accept struct {
}

type Received struct {
	protocol.Protocol
}

func (state *AcceptorActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *Listen:
		state.Listener, _ = net.Listen("tcp", fmt.Sprintf(":%d", msg.Port))
		context.ActorSystem().Root.Send(context.Self(), &Accept{})
		log.Printf("CRASH SERVER IS RUNNING : %d", msg.Port)

	case *Accept:
		conn, _ := state.Listener.Accept()

		props := actor.PropsFromProducer(func() actor.Actor {
			return &SessionActor{
				queue: make([]byte, 0),
			}
		})

		session := context.Spawn(props)
		context.ActorSystem().Root.Send(session, &SetConn{Conn: conn})
		context.ActorSystem().Root.Send(context.Self(), &Accept{})
	}
}
