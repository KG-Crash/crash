package main

import (
	"fmt"
	"log"
	"network"

	"protocol"
	"protocol/request"
	"protocol/response"
)

func OnCreateRoom(session *network.Session, x *request.CreateRoom) {
	session.Write(&response.CreateRoom{
		Id: 123,
	})
}

func OnJoinRoom(session *network.Session, x *request.JoinRoom) {
	session.Write(&response.JoinRoom{
		Users: []uint64{1, 2, 3, 4, 5},
	})
}

func main() {
	log.SetFlags(log.Ldate | log.Ltime)

	acceptor := network.NewAcceptor(
		func(session *network.Session, p protocol.Protocol) {

			switch msg := p.(type) {
			case *request.CreateRoom:
				OnCreateRoom(session, msg)

			case *request.JoinRoom:
				OnJoinRoom(session, msg)

			case *request.LeaveRoom:
				fmt.Println(msg)

			case *request.KickRoom:
				fmt.Println(msg)
			}

		}, func(session *network.Session) {

		})

	port := uint16(8000)
	log.Printf("CRASH SERVER IS RUNNING : %d", port)

	acceptor.Run(port)
}
