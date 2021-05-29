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

var game *actor.PID

func OnReceived(context actor.Context, user *model.UserActor, protocol protocol.Protocol) {
	switch protocol.(type) {
	case *request.CreateRoom:
		context.Send(game, &model.SpawnRoom{
			Master: context.Self(),
		})

	case *request.JoinRoom:
		context.Send(context.Self(), &network.Write{
			Protocol: &response.JoinRoom{
				Users: make([]string, 0),
			},
		})
	}
}

func OnAccepted(context actor.Context, acceptor *network.AcceptorActor, conn net.Conn) {
	context.Send(game, &model.SpawnUser{
		Conn:       conn,
		OnReceived: OnReceived,
	})
}

func main() {
	log.SetFlags(log.Ldate | log.Ltime)

	system := actor.NewActorSystem()

	game = system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return model.NewGame()
	}))

	acceptor := system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return &network.AcceptorActor{}
	}))
	system.Root.Send(acceptor, &network.BindAcceptor{
		OnAccepted: OnAccepted,
	})
	system.Root.Send(acceptor, &network.Listen{Port: 8000})

	_, _ = console.ReadLine()
}
