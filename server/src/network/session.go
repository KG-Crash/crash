package network

import (
	"encoding/binary"
	"io"
	"net"
	"protocol"
	"protocol/request"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type SessionActor struct {
	net.Conn
	queue []byte
}

type SetConn struct {
	net.Conn
}

type Receive struct {
}

type Write struct {
	protocol.Protocol
}

func (state *SessionActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *SetConn:
		state.Conn = msg.Conn
		context.ActorSystem().Root.Send(context.Self(), &Receive{})

	case *Receive:
		buffer := make([]byte, 4096)
		numRead, err := state.Conn.Read(buffer)
		if err == io.EOF {
			state.Conn.Close()
			context.Stop(context.Self())
			return
		}
		state.queue = append(state.queue, buffer[:numRead]...)

		for len(state.queue) > 0 {
			offset := 0
			size := binary.LittleEndian.Uint32(state.queue[offset:4])
			offset += 4

			if uint32(len(state.queue[offset:])) < size {
				break
			}

			deserialized := request.Deserialize(size-4, state.queue[offset:])
			context.Send(context.Parent(), &Received{Protocol: deserialized})

			state.queue = state.queue[size+4:]
		}

		context.ActorSystem().Root.Send(context.Self(), &Receive{})

	case *Write:
		serialized := msg.Protocol.Serialize()
		size := uint32(len(serialized))

		bytes := make([]byte, 8, 8+size)
		binary.LittleEndian.PutUint32(bytes[:], uint32(4+len(serialized)))

		identity := uint32(msg.Protocol.Identity())
		binary.LittleEndian.PutUint32(bytes[4:], identity)

		bytes = append(bytes, serialized...)
		state.Conn.Write(bytes)

		// log.Printf("[%s] Response %s : %s", state.Host(), response.Text(msg.Protocol), msg.Protocol)
	}
}
