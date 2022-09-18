package main

import (
	"KG/context"
	"KG/handler"
	"KG/model"
	"fmt"
	"log"
	"net"
	"os"
	"protocol/response"
)

func main() {

	port := 8000

	listen, err := net.Listen("tcp", fmt.Sprintf(":%d", port))
	if err != nil {
		os.Exit(1)
	}
	defer listen.Close()

	ctx := context.New()
	ist := handler.New()
	handler.Register(ist, ctx.OnRoomList)
	handler.Register(ist, ctx.OnCreateRoom)
	handler.Register(ist, ctx.OnEnterRoom)
	handler.Register(ist, ctx.OnLeaveRoom)
	handler.Register(ist, ctx.OnChat)
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
		ctx.Sessions[session.ID()] = session
		session.Send(response.Login{
			Id: session.ID(),
		})
		go session.Loop()
	}
}
