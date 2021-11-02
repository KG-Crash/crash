package main

import (
	"context"
	"fmt"
	"log"
	"os"
	"os/signal"

	"model/game"

	"github.com/AsynkronIT/protoactor-go/actor"
	"github.com/go-redis/redis"
)

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

	system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return game.New(Configuration.Port)
	}))

	// Subscribe to signal to finish interaction
	finish := make(chan os.Signal, 1)
	signal.Notify(finish, os.Interrupt, os.Kill)
	<-finish
}
