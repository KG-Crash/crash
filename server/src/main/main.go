package main

import (
	"log"
	"model"
	"net"
	"network"
	"os"
	"protocol"
	"protocol/request"
	"strconv"

	console "github.com/AsynkronIT/goconsole"
	"github.com/AsynkronIT/protoactor-go/actor"
)

var game *actor.PID

func OnReceived(context actor.Context, user *model.UserActor, protocol protocol.Protocol) {
	switch msg := protocol.(type) {
	case *request.CreateRoom:
		context.Send(game, &model.SpawnRoom{
			Master: context.Self(),
			UserId: user.Id,
		})

	case *request.JoinRoom:
		context.Send(game, &model.JoinRoom{
			User:   context.Self(),
			UserId: user.Id,
			RoomId: msg.Id,
		})

	case *request.LeaveRoom: // game > user > room
		context.Send(context.Self(), &model.LeaveRoom{
			UserId: user.Id,
			User:   context.Self(),
		})

	case *request.RoomList:
		context.Send(game, &model.RoomList{
			User: context.Self(),
		})

	case *request.Chat:
		context.Send(context.Self(), &model.Chat{
			UserId:  user.Id,
			Message: msg.Message,
		})

	case *request.Whisper:
		context.Send(game, &model.Whisper{
			From:    user.Id,
			To:      msg.User,
			Message: msg.Message,
		})

	case *request.KickRoom:
		context.Send(context.Self(), &model.Kick{
			From: user.Id,
			To:   msg.User,
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

	args := os.Args[1:]
	port := args[0]

	if port == "" {
		port = "8000"
	}

	p, err := strconv.Atoi(port)
	if err != nil {
		log.Fatalf("Port must be unsigned short type. %s does not port format.", port)
		return
	}

	system.Root.Send(acceptor, &network.Listen{Port: uint16(p)})
	_, _ = console.ReadLine()
}
