package main

import (
	"KG/context"
	"KG/handler"
	"KG/session"
	"fmt"
	"log"
	"net"
	"os"
	"protocol/request"
	"protocol/response"
)

func OnChat(session *session.Session, req request.Chat) {
	session.Send(response.Chat{
		User:    "",
		Message: "",
	})
}

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
	handler.Register(handler_ist, OnChat)

	for {
		conn, err := listen.Accept()
		if err != nil {
			log.Fatalln(err)
			continue
		}

		session := session.New(conn, handler_ist)
		session.Send(response.Login{
			Id: session.ID(),
		})
		go session.Loop()
	}
}
