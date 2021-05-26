package main

import (
	"fmt"
	"log"
	"network"

	console "github.com/AsynkronIT/goconsole"
	"github.com/AsynkronIT/protoactor-go/actor"
)

func main() {
	log.SetFlags(log.Ldate | log.Ltime)

	system := actor.NewActorSystem()
	props := actor.PropsFromProducer(func() actor.Actor {
		return &network.AcceptorActor{}
	}).WithReceiverMiddleware(func(next actor.ReceiverFunc) actor.ReceiverFunc {
		fn := func(c actor.ReceiverContext, env *actor.MessageEnvelope) {
			switch msg := env.Message.(type) {
			case *network.Received:
				fmt.Println(msg.Protocol)
			}

			next(c, env)
		}

		return fn
	})

	pid := system.Root.Spawn(props)
	system.Root.Send(pid, &network.Listen{Port: 8000})
	_, _ = console.ReadLine()
}
