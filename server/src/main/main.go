package main

import (
	"log"
	"network"

	"protocol"
	"protocol/request"
	"protocol/response"
)

var handler map[uint32]func(*network.Session, protocol.Protocol)

func init() {
	handler = make(map[uint32]func(*network.Session, protocol.Protocol))
	handler[request.CREATE_ROOM] = func(s *network.Session, p protocol.Protocol) {
		OnCreateRoom(s, p.(*request.CreateRoom))
	}
	handler[request.JOIN_ROOM] = func(s *network.Session, p protocol.Protocol) {
		OnJoinRoom(s, p.(*request.JoinRoom))
	}
}

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
		func(session *network.Session, identity uint32, p protocol.Protocol) {
			handler[identity](session, p)
		}, func(session *network.Session) {

		})

	port := uint16(8000)
	log.Printf("CRASH SERVER IS RUNNING : %d", port)

	acceptor.Run(port)
}
