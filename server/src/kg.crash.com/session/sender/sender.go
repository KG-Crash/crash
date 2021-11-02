package sender

import (
	"encoding/binary"
	"net"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type Actor struct {
	net.Conn
}

func New(conn net.Conn) *Actor {
	return &Actor{
		Conn: conn,
	}
}

func (state *Actor) Receive(context actor.Context) {
	switch x := context.Message().(type) {
	case *Send:
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
