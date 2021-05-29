package network

import (
	"encoding/binary"
	"net"
	"protocol"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type SenderActor struct {
	net.Conn
}

type Write struct {
	protocol.Protocol
}

func NewSenderActor() *SenderActor {
	return &SenderActor{}
}

func (state *SenderActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *SetConnection:
		state.Conn = msg.Conn

	case *Write:
		serialized := msg.Protocol.Serialize()
		size := uint32(len(serialized))

		bytes := make([]byte, 8, 8+size)
		binary.LittleEndian.PutUint32(bytes[:], uint32(4+len(serialized)))

		identity := uint32(msg.Protocol.Identity())
		binary.LittleEndian.PutUint32(bytes[4:], identity)

		bytes = append(bytes, serialized...)
		state.Conn.Write(bytes)
	}
}
