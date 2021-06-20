package main

import (
	"context"
	"fmt"
	"log"
	"model"
	"msg"
	"net"
	"network"
	"os"
	"protocol"
	"protocol/request"
	"strconv"

	console "github.com/AsynkronIT/goconsole"
	"github.com/AsynkronIT/protoactor-go/actor"
	"github.com/go-redis/redis"
)

var game *actor.PID

func OnReceived(context actor.Context, id string, protocol protocol.Protocol) {
	switch x := protocol.(type) {
	case *request.CreateRoom:
		context.Send(game, &msg.SpawnRoom{
			Master: context.Self(),
			UserId: id,
		})

	case *request.JoinRoom:
		context.Send(game, &msg.JoinRoom{
			User:   context.Self(),
			UserId: id,
			RoomId: x.Id,
		})

	case *request.LeaveRoom: // game > user > room
		context.Send(context.Self(), &msg.LeaveRoom{
			UserId: id,
			User:   context.Self(),
		})

	case *request.RoomList:
		context.Send(game, &msg.RoomList{
			User: context.Self(),
		})

	case *request.Chat:
		context.Send(context.Self(), &msg.Chat{
			UserId:  id,
			Message: x.Message,
		})

	case *request.Whisper:
		context.Send(game, &msg.Whisper{
			From:    id,
			To:      x.User,
			Message: x.Message,
		})

	case *request.KickRoom:
		context.Send(context.Self(), &msg.Kick{
			From: id,
			To:   x.User,
		})
	}
}

func OnAccepted(context actor.Context, conn net.Conn) {
	context.Send(game, &msg.SpawnUser{
		Conn:       conn,
		OnReceived: OnReceived,
	})
}

func main() {
	ctx := context.Background()
	client := redis.NewClient(&redis.Options{
		Addr: "192.168.0.180:6379",
	})

	pong, err := client.Ping(ctx).Result()
	fmt.Println(pong, err)
	client.Close()

	log.SetFlags(log.Ldate | log.Ltime)

	system := actor.NewActorSystem()

	game = system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return model.NewGame()
	}))

	acceptor := system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return &network.AcceptorActor{}
	}))
	system.Root.Send(acceptor, &msg.BindAcceptor{
		OnAccepted: OnAccepted,
	})

	port := "8000"
	args := os.Args[1:]
	if len(args) > 0 {
		port = args[0]
	}

	p, err := strconv.Atoi(port)
	if err != nil {
		log.Fatalf("Port must be unsigned short type. %s does not port format.", port)
		return
	}

	system.Root.Send(acceptor, &msg.Listen{Port: uint16(p)})
	_, _ = console.ReadLine()
}
