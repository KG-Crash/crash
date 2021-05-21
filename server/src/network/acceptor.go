package network

import (
	"fmt"
	"io"
	"log"
	"net"
	"protocol"
)

type Acceptor struct {
	sessions             map[*Session]*Session
	callbackReceive      func(*Session, uint32, protocol.Protocol)
	callbackDisconnected func(*Session)
}

func NewAcceptor(callbackReceive func(*Session, uint32, protocol.Protocol), callbackDisconnected func(*Session)) *Acceptor {
	return &Acceptor{
		sessions:             make(map[*Session]*Session),
		callbackReceive:      callbackReceive,
		callbackDisconnected: callbackDisconnected,
	}
}

func (acceptor *Acceptor) onAccept(session *Session) {

	buffer := make([]byte, 512)
	for {
		err := session.Read(buffer)
		if err == io.EOF {
			delete(acceptor.sessions, session)
			return
		}
	}
}

func (acceptor *Acceptor) Run(port uint16) {
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

		session := NewSession(conn, acceptor.callbackReceive, acceptor.callbackDisconnected)
		acceptor.sessions[&session] = &session
		go acceptor.onAccept(&session)
	}
}
