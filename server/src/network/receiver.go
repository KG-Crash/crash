package network

import (
	"encoding/binary"
	"io"
	"net"
	"protocol"
	"protocol/request"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type ReceiverActor struct {
	net.Conn
	Queue []byte
}

type Received struct {
	protocol.Protocol
}

func NewReceiverActor() *ReceiverActor {
	return &ReceiverActor{
		Queue: make([]byte, 0, 4096),
	}
}

func (state *ReceiverActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *SetConnection:
		state.Conn = msg.Conn
		context.Send(context.Self(), &Receive{})

	case *Receive:
		buffer := make([]byte, 4096)
		numRead, err := state.Conn.Read(buffer)
		if err == io.EOF {
			context.Stop(context.Parent())
			return
		}
		state.Queue = append(state.Queue, buffer[:numRead]...)

		for len(state.Queue) > 0 {
			offset := 0
			size := binary.LittleEndian.Uint32(state.Queue[offset:4])
			offset += 4

			if uint32(len(state.Queue[offset:])) < size {
				break
			}

			deserialized := request.Deserialize(size-4, state.Queue[offset:])

			context.Send(context.Parent(), &Received{
				Protocol: deserialized,
			})

			state.Queue = state.Queue[size+4:]
		}

		context.Send(context.Self(), &Receive{})
	}
}
