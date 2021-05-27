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

var sender *actor.PID

func OnReceiveProtocol(c *actor.RootContext, conn net.Conn, p protocol.Protocol) {
	switch p.(type) {
	case *request.CreateRoom:
		c.Send(sender, &network.Write{
			Conn: conn,
			Protocol: &response.CreateRoom{
				Id: 1000,
			},
		})

	case *request.JoinRoom:
		c.Send(sender, &network.Write{
			Conn: conn,
			Protocol: &response.JoinRoom{
				Users: make([]uint64, 0),
			},
		})
	}
}

func OnSessionMiddleware(next actor.ReceiverFunc) actor.ReceiverFunc {
	fn := func(c actor.ReceiverContext, env *actor.MessageEnvelope) {
		switch msg := env.Message.(type) {
		case *network.Received:
			OnReceiveProtocol(c.ActorSystem().Root, msg.Conn, msg.Protocol)
		}

		next(c, env)
	}

	return fn
}

func OnAcceptorMiddleware(next actor.ReceiverFunc) actor.ReceiverFunc {
	fn := func(c actor.ReceiverContext, env *actor.MessageEnvelope) {
		switch msg := env.Message.(type) {
		case *network.Accepted:
			props := actor.PropsFromProducer(func() actor.Actor {
				return &model.SessionActor{
					Queue: make([]byte, 0),
				}
			}).WithReceiverMiddleware(OnSessionMiddleware)

			root := c.ActorSystem().Root
			session := root.Spawn(props)
			root.Send(session, &model.SetConn{Conn: msg.Conn})
		}

		next(c, env)
	}

	return fn
}

func main() {
	log.SetFlags(log.Ldate | log.Ltime)

	system := actor.NewActorSystem()

	sender = system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return &network.SenderActor{}
	}))

	pid := system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return &network.AcceptorActor{}
	}).WithReceiverMiddleware(OnAcceptorMiddleware))
	system.Root.Send(pid, &network.Listen{Port: 8000})

	_, _ = console.ReadLine()
}
