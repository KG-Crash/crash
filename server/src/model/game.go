package model

import (
	"fmt"
	"net"
	"network"

	"github.com/AsynkronIT/protoactor-go/actor"
	"github.com/google/uuid"
)

type GameActor struct {
	Rooms map[string]*actor.PID
	Users map[string]*actor.PID
}

func NewGame() *GameActor {
	return &GameActor{
		Rooms: make(map[string]*actor.PID),
		Users: make(map[string]*actor.PID),
	}
}

type SpawnUser struct {
	net.Conn
	OnReceived
}

type SpawnRoom struct {
	Master *actor.PID
}

func (state *GameActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *SpawnUser:
		id := uuid.NewString()
		props := actor.PropsFromProducer(func() actor.Actor {
			return &UserActor{
				Id: id,
			}
		})

		user := context.Spawn(props)
		state.Users[id] = user
		context.Send(user, &BindUser{
			OnReceived: msg.OnReceived,
		})
		context.Send(user, &network.SetConnection{Conn: msg.Conn})

	case *SpawnRoom:
		id := uuid.NewString()
		props := actor.PropsFromProducer(func() actor.Actor {
			return NewRoom(id, msg.Master)
		})
		room := context.Spawn(props)
		state.Rooms[id] = room

		context.Send(msg.Master, &JoinRoom{
			Room: room,
		})

	default:
		fmt.Print(msg)
	}
}
