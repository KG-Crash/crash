package game

import (
	"fmt"
	"model/room"
	"model/user"
	"msg"

	"github.com/AsynkronIT/protoactor-go/actor"
	"github.com/google/uuid"
	"kg.crash.com/acceptor"
)

type Actor struct {
	port     uint16
	acceptor *actor.PID

	Rooms map[string]*actor.PID
	Users map[string]*actor.PID
}

func New(port uint16) *Actor {
	return &Actor{
		port: port,

		Rooms: make(map[string]*actor.PID),
		Users: make(map[string]*actor.PID),
	}
}

func (state *Actor) Receive(ctx actor.Context) {
	switch x := ctx.Message().(type) {
	case *actor.Started:
		state.acceptor = ctx.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return acceptor.New(state.port)
		}))

	case *acceptor.Connected:
		id := uuid.NewString()
		state.Users[id] = ctx.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return user.New(id, x.Conn)
		}))
		return

	case *msg.Disconnected:
		delete(state.Users, x.ID)

	case *msg.RequestGetUser:
		user, ok := state.Users[x.ID]
		if !ok {
			user = nil
		}
		ctx.Respond(&msg.ResponseGetUser{User: user})

	case *msg.RequestGetRoom:
		room, ok := state.Rooms[x.ID]
		if !ok {
			room = nil
		}
		ctx.Respond(&msg.ResponseGetRoom{Room: room})

	case *msg.RequestGetRoomList:
		response := &msg.ResponseGetRoomList{
			Rooms: *actor.NewPIDSet(),
		}
		for _, room := range state.Rooms {
			response.Rooms.Add(room)
		}
		ctx.Respond(response)

	case *msg.RequestCreateRoom:
		id := uuid.NewString()
		room := ctx.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return room.New(id, state.Users[x.ID], room.RoomConfig{
				Team:  x.Teams,
				Title: x.Title,
			})
		}))
		state.Rooms[id] = room

		ctx.Respond(&msg.ResponseCreateRoom{
			ID:   id,
			Room: room,
		})

	case *msg.DestroyRoom:
		_, exists := state.Rooms[x.ID]
		if !exists {
			return
		}
		delete(state.Rooms, x.ID)

	default:
		fmt.Print(x)
	}
}
