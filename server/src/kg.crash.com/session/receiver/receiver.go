package receiver

import (
	"encoding/binary"
	"io"
	"net"
	"protocol/request"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type Actor struct {
	net.Conn
	Queue []byte
}

func New(conn net.Conn) *Actor {
	return &Actor{
		Conn:  conn,
		Queue: make([]byte, 0, 4096),
	}
}

func (state *Actor) Receive(ctx actor.Context) {
	switch ctx.Message().(type) {
	case *actor.Started:
		ctx.Send(ctx.Self(), &receive{})

	case *receive:
		buffer := make([]byte, 4096)
		numRead, err := state.Conn.Read(buffer)
		if numRead == 0 || err == io.EOF {
			ctx.Send(ctx.Parent(), &Disconnected{})
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

			ctx.Send(ctx.Parent(), &Received{
				Protocol: deserialized,
			})

			state.Queue = state.Queue[size+4:]
		}

		ctx.Send(ctx.Self(), &receive{})
	}
}
