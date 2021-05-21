package network

import (
	"encoding/binary"
	"io"
	"log"
	"net"
	"protocol"
	"protocol/request"
	"protocol/response"
)

type Session struct {
	conn                 net.Conn
	queue                []byte
	output               chan protocol.Protocol
	callbackReceive      func(*Session, uint32, protocol.Protocol)
	callbackDisconnected func(*Session)
}

func NewSession(conn net.Conn, callbackReceive func(*Session, uint32, protocol.Protocol), callbackDisconnected func(*Session)) Session {
	session := Session{
		conn:                 conn,
		queue:                make([]byte, 0, 4096),
		output:               make(chan protocol.Protocol),
		callbackReceive:      callbackReceive,
		callbackDisconnected: callbackDisconnected,
	}

	go session.processWrite()

	return session
}

func (session *Session) Host() string {
	return session.conn.RemoteAddr().String()
}

func (session *Session) processWrite() {
	for {
		p := <-session.output
		serialized := p.Serialize()
		size := uint32(len(serialized))

		bytes := make([]byte, 8, 8+size)
		binary.LittleEndian.PutUint32(bytes[:], uint32(4+len(serialized)))

		identity := uint32(p.Identity())
		binary.LittleEndian.PutUint32(bytes[4:], identity)

		bytes = append(bytes, serialized...)
		session.conn.Write(bytes)

		log.Printf("[%s] Response %s : %s", session.Host(), response.Text[identity], p)
	}
}

func (session *Session) Read(buffer []byte) error {
	numRead, err := session.conn.Read(buffer)
	if err == io.EOF {
		session.callbackDisconnected(session)
		return io.EOF
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
		deserialized := request.Allocator[identity](payload)

		log.Printf("[%s] Request %s : %s", session.Host(), request.Text[identity], deserialized)
		session.callbackReceive(session, identity, deserialized)
		session.queue = session.queue[size+4:]
	}

	return nil
}

func (session *Session) Write(p protocol.Protocol) {
	session.output <- p
}
