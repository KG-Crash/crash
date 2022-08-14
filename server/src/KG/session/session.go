package session

import (
	"encoding/binary"
	"io"
	"net"
)

type Session struct {
	net.Conn
	queue []byte
}

func New(conn net.Conn) *Session {
	return &Session{
		Conn:  conn,
		queue: make([]byte, 0, 4096),
	}
}

func (session *Session) Handler() {
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

			session.queue = session.queue[size+4:]
		}
	}
}
