package session

import (
	"KG/handler"
	"encoding/binary"
	"io"
	"net"
	"protocol/request"
)

type Session struct {
	net.Conn
	queue   []byte
	handler *handler.Handler
}

func New(conn net.Conn, handler *handler.Handler) *Session {
	return &Session{
		Conn:    conn,
		queue:   make([]byte, 0, 4096),
		handler: handler,
	}
}

func (session *Session) Loop() {
	for {
		buffer := make([]byte, 4096)
		numRead, err := session.Conn.Read(buffer)
		if numRead == 0 || err == io.EOF {
			// disconnected
			break
		}

		session.queue = append(session.queue, buffer[:numRead]...)
		for len(session.queue) > 0 {
			offset := 0
			size := binary.LittleEndian.Uint32(session.queue[offset:4])
			offset += 4

			if uint32(len(session.queue[offset:])) < size {
				break
			}

			deserialized := request.Deserialize(size-4, session.queue[offset:])
			session.handler.Channel <- handler.Message{
				Session:  session,
				Protocol: deserialized}
			session.queue = session.queue[size+4:]
		}
	}
}
