package model

import (
	"fmt"
	"log"
	"msg"
	"protocol/response"

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

func (state *GameActor) UserIdList() []string {
	result := make([]string, 0, len(state.Users))
	for id, _ := range state.Users {
		result = append(result, id)
	}

	return result
}

func (state *GameActor) RoomIdList() []string {
	result := make([]string, 0, len(state.Rooms))
	for id := range state.Rooms {
		result = append(result, id)
	}

	return result
}

func (state *GameActor) OnSpawnUser(context actor.Context, x *msg.SpawnUser) {
	id := uuid.NewString()
	if _, ok := state.Users[id]; ok {
		// TODO : disconnect
		return
	}

	props := actor.PropsFromProducer(func() actor.Actor {
		return NewUser(id)
	})

	user := context.Spawn(props)
	state.Users[id] = user
	context.Send(user, &msg.BindUser{
		Id:         id,
		OnReceived: x.OnReceived,
	})
	context.Send(user, &msg.SetConnection{Conn: x.Conn})
}

func (state *GameActor) OnSpawnRoom(context actor.Context, x *msg.SpawnRoom) {
	// TODO : 유저가 이미 방에 참여중인 상태에서 방을 생성하려는 시도를 할 수 있음

	id := uuid.NewString()
	if _, ok := state.Rooms[id]; ok {
		context.Send(x.Master, &response.CreateRoom{Error: 1})
		return
	}

	props := actor.PropsFromProducer(func() actor.Actor {
		return NewRoom(id, x.Master)
	})
	room := context.Spawn(props)
	state.Rooms[id] = room

	context.Send(room, &msg.JoinRoom{
		User:   x.Master,
		UserId: x.UserId,
		RoomId: id,
	})

	log.Printf("Spawn Room [%s]", id)
}

func (state *GameActor) OnDestroyedRoom(context actor.Context, x *msg.DestroyedRoom) {
	if _, ok := state.Rooms[x.RoomId]; !ok {
		return
	}

	delete(state.Rooms, x.RoomId)
	log.Printf("Destroyed Room [%s]", x.RoomId)
}

func (state *GameActor) OnRoomList(context actor.Context, x *msg.RoomList) {
	context.Send(x.User, &response.RoomList{
		Rooms: state.RoomIdList(),
	})
}

func (state *GameActor) OnJoinRoom(context actor.Context, x *msg.JoinRoom) {
	room, ok := state.Rooms[x.RoomId]
	if !ok {
		context.Send(x.User, &msg.JoinedRoom{
			Error: 1,
		})
		return
	}

	context.Send(room, &msg.JoinRoom{
		User:   x.User,
		UserId: x.UserId,
	})
	log.Printf("Request join user [%s] into Room [%s]", x.UserId, x.RoomId)
}

func (state *GameActor) OnLogout(context actor.Context, x *msg.Logout) {
	_, ok := state.Users[x.UserId]
	if !ok {
		return
	}

	delete(state.Users, x.UserId)
	log.Printf("User [%s] logout", x.UserId)
}

func (state *GameActor) OnWhisper(context actor.Context, x *msg.Whisper) {
	from, from_ok := state.Users[x.From]
	if !from_ok {
		return
	}

	to, to_ok := state.Users[x.To]
	if !to_ok {
		context.Send(from, &response.Whisper{Error: 1})
		return
	}

	context.Send(to, &response.Whisper{
		User:    x.From,
		Message: x.Message,
		Error:   0,
	})
	log.Printf("User [%s] whisper to user [%s] : %s", x.From, x.To, x.Message)
}

func (state *GameActor) Receive(context actor.Context) {
	switch x := context.Message().(type) {
	case *msg.SpawnUser:
		state.OnSpawnUser(context, x)

	case *msg.SpawnRoom:
		state.OnSpawnRoom(context, x)

	case *msg.DestroyedRoom:
		state.OnDestroyedRoom(context, x)

	case *msg.RoomList:
		state.OnRoomList(context, x)

	case *msg.JoinRoom:
		state.OnJoinRoom(context, x)

	case *msg.Logout:
		state.OnLogout(context, x)

	case *msg.Whisper:
		state.OnWhisper(context, x)

	default:
		fmt.Print(x)
	}
}
