package acceptor

import (
	"fmt"
	"log"
	"net"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type Actor struct {
	net.Listener
	Port uint16
}

func New(port uint16) *Actor {
	return &Actor{
		Port: port,
	}
}

func (state *Actor) Receive(context actor.Context) {
	switch context.Message().(type) {
	case *actor.Started:
		state.Listener, _ = net.Listen("tcp", fmt.Sprintf(":%d", state.Port))
		context.ActorSystem().Root.Send(context.Self(), &accept{})
		log.Printf("CRASH SERVER IS RUNNING : %d", state.Port)

	case *accept:
		conn, _ := state.Listener.Accept()
		context.Send(context.Parent(), &Connected{Conn: conn})
		context.Send(context.Self(), &accept{})
	}
}
