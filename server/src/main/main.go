package main

import (
	"KG/handler"
	"KG/session"
	"fmt"
	"log"
	"net"
	"os"
	"protocol/request"
)

func OnRoomList(session *session.Session, req request.RoomList) {

	fmt.Println(req)
}

func OnChat(session *session.Session, req request.Chat) {
	fmt.Println(req)
}

func main() {

	port := 8000

	listen, err := net.Listen("tcp", fmt.Sprintf(":%d", port))
	if err != nil {
		os.Exit(1)
	}
	defer listen.Close()

	handler_ist := handler.New()
	handler.Register(handler_ist, OnRoomList)
	handler.Register(handler_ist, OnChat)

	for {
		conn, err := listen.Accept()
		if err != nil {
			log.Fatalln(err)
			continue
		}

		sesion := session.New(conn, handler_ist)
		go sesion.Loop()
	}
}
