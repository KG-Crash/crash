package network

import (
	"encoding/binary"
	"msg"
	"net"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type SenderActor struct {
	net.Conn
}

func NewSenderActor() *SenderActor {
	return &SenderActor{}
}

func (state *SenderActor) Receive(context actor.Context) {
	switch x := context.Message().(type) {
	case *msg.SetConnection:
		state.Conn = x.Conn

	case *msg.Write:
		serialized := x.Protocol.Serialize()
		size := uint32(len(serialized))

		bytes := make([]byte, 8, 8+size)
		binary.LittleEndian.PutUint32(bytes[:], uint32(4+len(serialized)))

		identity := uint32(x.Protocol.Identity())
		binary.LittleEndian.PutUint32(bytes[4:], identity)

		bytes = append(bytes, serialized...)
		state.Conn.Write(bytes)
	}
}
