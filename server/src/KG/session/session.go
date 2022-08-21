package session

import (
	"KG/handler"
	"encoding/binary"
	"io"
	"net"
	"protocol"
	"protocol/request"

	"github.com/google/uuid"
)

type Session struct {
	net.Conn
	queue   []byte
	handler *handler.Handler
	id      string
}

func New(conn net.Conn, handler *handler.Handler) *Session {
	return &Session{
		Conn:    conn,
		queue:   make([]byte, 0, 4096),
		handler: handler,
		id:      uuid.NewString(),
	}
}

func (session *Session) ID() string {
	return session.id
}

func (session *Session) Send(res protocol.Protocol) {
	serialized := res.Serialize()
	size := uint32(len(serialized))

	bytes := make([]byte, 8, 8+size)
	binary.LittleEndian.PutUint32(bytes[:], uint32(4+len(serialized)))

	identity := uint32(res.Identity())
	binary.LittleEndian.PutUint32(bytes[4:], identity)

	bytes = append(bytes, serialized...)
	session.Conn.Write(bytes)
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
