package main

import (
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
	acceptor := network.NewAcceptor(
		func(session *network.Session, identity uint32, payload []byte) {
			deserialized := request.Allocator[identity](payload)
			handler[identity](session, deserialized)
		}, func(session *network.Session) {

		})

	acceptor.Run(8000)
}
