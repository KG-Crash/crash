package main

import (
	"fmt"
	"log"
	"net"
	"os"

	"github.com/KG-Crash/crash/KG/handler"
	"github.com/KG-Crash/crash/game/context"
	"github.com/KG-Crash/crash/model"
)

func main() {

	port := 8000

	listen, err := net.Listen("tcp", fmt.Sprintf(":%d", port))
	if err != nil {
		os.Exit(1)
	}
	defer listen.Close()
	fmt.Println("server is running")

	ctx := context.New()
	ist := handler.New()
	handler.Register(ist, ctx.OnLogin)
	handler.Register(ist, ctx.OnRoomList)
	handler.Register(ist, ctx.OnCreateRoom)
	handler.Register(ist, ctx.OnEnterRoom)
	handler.Register(ist, ctx.OnLeaveRoom)
	handler.Register(ist, ctx.OnChat)
	handler.Register(ist, ctx.OnGameChat)
	handler.Register(ist, ctx.OnGameStart)
	handler.Register(ist, ctx.OnReady)
	handler.Register(ist, ctx.OnAction)
	handler.Register(ist, ctx.OnWhisper)
	handler.Register(ist, ctx.OnKick)
	handler.RegisterExit(ist, ctx.OnExit)

	for {
		conn, err := listen.Accept()
		if err != nil {
			log.Fatalln(err)
			continue
		}

		session := model.NewSession(conn, ist)
		go session.Loop()
	}
}
