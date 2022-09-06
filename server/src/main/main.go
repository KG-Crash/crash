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

	handler_ist := handler.New()
	handler.Register(handler_ist, ctx.OnRoomList)
	handler.Register(handler_ist, ctx.OnCreateRoom)
	handler.Register(handler_ist, ctx.OnEnterRoom)
	handler.Register(handler_ist, ctx.OnLeaveRoom)
	handler.Register(handler_ist, ctx.OnChat)
	handler.Register(handler_ist, ctx.OnGameStart)
	handler.Register(handler_ist, ctx.OnReady)
	handler.Register(handler_ist, ctx.OnAction)

	for {
		conn, err := listen.Accept()
		if err != nil {
			log.Fatalln(err)
			continue
		}

		session := model.NewSession(conn, handler_ist)
		session.Send(response.Login{
			Id: session.ID(),
		})
		go session.Loop()
	}
}
