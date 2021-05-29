package main

import (
	"log"
	"model"
	"net"
	"network"
	"protocol"
	"protocol/request"
	"protocol/response"

	console "github.com/AsynkronIT/goconsole"
	"github.com/AsynkronIT/protoactor-go/actor"
)

func OnReceived(context actor.Context, user *model.UserActor, protocol protocol.Protocol) {
	switch protocol.(type) {
	case *request.CreateRoom:
		context.Send(context.Self(), &network.Write{
			Protocol: &response.CreateRoom{
				Id: 1000,
			},
		})

	case *request.JoinRoom:
		context.Send(context.Self(), &network.Write{
			Protocol: &response.JoinRoom{
				Users: make([]uint64, 0),
			},
		})
	}
}

func OnAccepted(context actor.Context, acceptor *network.AcceptorActor, conn net.Conn) {
	props := actor.PropsFromProducer(func() actor.Actor {
		return &model.UserActor{}
	})

	user := context.Spawn(props)
	context.Send(user, &model.BindUser{
		OnReceived: OnReceived,
	})
	context.Send(user, &network.SetConn{Conn: conn})
}

func main() {
	log.SetFlags(log.Ldate | log.Ltime)

	system := actor.NewActorSystem()

	pid := system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return &network.AcceptorActor{}
	}))
	system.Root.Send(pid, &network.BindAcceptor{
		OnAccepted: OnAccepted,
	})
	system.Root.Send(pid, &network.Listen{Port: 8000})

	_, _ = console.ReadLine()
}
