package main

import (
	"encoding/binary"
	"fmt"
	"io"
	"log"
	"net"

	Protocol "protocol"
	Request "protocol/Request"
	Response "protocol/Response"
)

type Session struct {
	conn  net.Conn
	queue []byte
}

func NewSession(conn net.Conn) Session {
	return Session{
		conn:  conn,
		queue: make([]byte, 0, 4096),
	}
}

func (session *Session) Write(p Protocol.Protocol) {
	serialized := p.Serialize()
	size := uint32(len(serialized))

	bytes := make([]byte, 8, 8+size)
	binary.LittleEndian.PutUint32(bytes[:], uint32(4+len(serialized)))

	identity := uint32(p.Identity())
	binary.LittleEndian.PutUint32(bytes[4:], identity)

	bytes = append(bytes, serialized...)
	session.conn.Write(bytes)
}

func OnCreateRoom(x *Response.CreateRoom) {
	fmt.Println(x)
}

func OnJoinRoom(x *Response.JoinRoom) {}

func DisconnectedHandler(session *Session) {

}

func ReceiveHandler(session *Session) {

	buffer := make([]byte, 512)
	for {
		numRead, err := session.conn.Read(buffer)
		if err == io.EOF {
			go DisconnectedHandler(session)
			return
		}

		session.queue = append(session.queue, buffer[:numRead]...)

		for len(session.queue) > 0 {
			offset := 0
			size := binary.LittleEndian.Uint32(session.queue[offset:4])
			offset += 4

			if uint32(len(session.queue[offset:])) < size {
				break
			}

			identity := binary.LittleEndian.Uint32(session.queue[offset : offset+4])
			offset += 4

			payload := session.queue[offset : offset+int(size)-4]

			switch identity {
			case Request.CREATE_ROOM:
				x := Request.CreateRoom{}
				x.Deserialize(payload)
				fmt.Println(x)

				session.Write(&Response.CreateRoom{
					Id: 10,
				})

			case Request.JOIN_ROOM:
				x := Request.JoinRoom{}
				x.Deserialize(payload)
				fmt.Println(x)

				session.Write(&Response.JoinRoom{})
			}

			session.queue = session.queue[size+4:]
		}
	}
}

func AcceptHandler(port int) {
	listener, err := net.Listen("tcp", fmt.Sprintf(":%d", port))
	if err != nil {
		log.Fatal(err)
		return
	}
	defer listener.Close()

	for {
		conn, err := listener.Accept()
		if err != nil {
			log.Print(err)
			continue
		}

		session := NewSession(conn)
		go ReceiveHandler(&session)
	}
}

func main() {
	// m := map[int]interface{}{
	// 	Response.CREATE_ROOM: OnCreateRoom,
	// 	Response.JOIN_ROOM:   OnJoinRoom,
	// }

	// id := Response.CREATE_ROOM
	// handler := m[id]

	// switch id {

	// case Response.CREATE_ROOM:
	// 	handler.(func(*Response.CreateRoom))(&Response.CreateRoom{})

	// case Response.JOIN_ROOM:
	// 	handler.(func(*Response.JoinRoom))(&Response.JoinRoom{})
	// }

	AcceptHandler(8000)
}
