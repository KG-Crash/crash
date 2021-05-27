package model

import (
	"encoding/binary"
	"io"
	"net"
	"network"
	"protocol/request"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type SessionActor struct {
	net.Conn
	Queue []byte
}

type SetConn struct {
	net.Conn
}

type Receive struct {
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
		state.Queue = append(state.Queue, buffer[:numRead]...)

		for len(state.Queue) > 0 {
			offset := 0
			size := binary.LittleEndian.Uint32(state.Queue[offset:4])
			offset += 4

			if uint32(len(state.Queue[offset:])) < size {
				break
			}

			deserialized := request.Deserialize(size-4, state.Queue[offset:])

			context.Send(context.Self(), &network.Received{
				Conn:     state.Conn,
				Protocol: deserialized,
			})

			state.Queue = state.Queue[size+4:]
		}

		context.Send(context.Self(), &Receive{})
	}
}
