package network

import (
	"encoding/binary"
	"io"
	"msg"
	"net"
	"protocol/request"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type ReceiverActor struct {
	net.Conn
	Queue []byte
}

func NewReceiverActor() *ReceiverActor {
	return &ReceiverActor{
		Queue: make([]byte, 0, 4096),
	}
}

func (state *ReceiverActor) Receive(context actor.Context) {
	switch x := context.Message().(type) {
	case *msg.SetConnection:
		state.Conn = x.Conn
		context.Send(context.Self(), &msg.Receive{})

	case *msg.Receive:
		buffer := make([]byte, 4096)
		numRead, err := state.Conn.Read(buffer)
		if numRead == 0 || err == io.EOF {
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

			context.Send(context.Parent(), &msg.Received{
				Protocol: deserialized,
			})

			state.Queue = state.Queue[size+4:]
		}

		context.Send(context.Self(), &msg.Receive{})
	}
}
