package main

import (
	"log"
	"os"
	"os/signal"
	"table"

	"model/game"

	"github.com/AsynkronIT/protoactor-go/actor"
)

func main() {
	log.SetFlags(log.Ldate | log.Ltime)

	system := actor.NewActorSystem()

	system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return game.New(Config.Port)
	}))

	// Subscribe to signal to finish interaction
	finish := make(chan os.Signal, 1)
	signal.Notify(finish, os.Interrupt, os.Kill)
	<-finish

	data := table.TableSample["hello"][0]
	print(data.MemberField1)
}
