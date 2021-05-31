package model

import (
	"fmt"
	"net"
	"network"
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

type SpawnUser struct {
	net.Conn
	OnReceived
}

type SpawnRoom struct {
	Master *actor.PID
	UserId string
}

type RoomList struct {
	User *actor.PID
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

func (state *GameActor) OnSpawnUser(context actor.Context, msg *SpawnUser) {
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
	context.Send(user, &BindUser{
		Id:         id,
		OnReceived: msg.OnReceived,
	})
	context.Send(user, &network.SetConnection{Conn: msg.Conn})
}

func (state *GameActor) OnSpawnRoom(context actor.Context, msg *SpawnRoom) {
	// TODO : 유저가 이미 방에 참여중인 상태에서 방을 생성하려는 시도를 할 수 있음

	id := uuid.NewString()
	if _, ok := state.Rooms[id]; ok {
		context.Send(msg.Master, &network.Write{
			Protocol: &response.CreateRoom{
				Error: 1,
			},
		})
		return
	}

	props := actor.PropsFromProducer(func() actor.Actor {
		return NewRoom(id, msg.Master)
	})
	room := context.Spawn(props)
	state.Rooms[id] = room

	context.Send(room, &JoinRoom{
		User:   msg.Master,
		UserId: msg.UserId,
	})
}

func (state *GameActor) OnDestroyedRoom(context actor.Context, msg *DestroyedRoom) {
	if _, ok := state.Rooms[msg.RoomId]; !ok {
		return
	}

	delete(state.Rooms, msg.RoomId)
}

func (state *GameActor) OnRoomList(context actor.Context, msg *RoomList) {
	context.Send(msg.User, &network.Write{
		Protocol: &response.RoomList{
			Rooms: state.RoomIdList(),
		},
	})
}

func (state *GameActor) Receive(context actor.Context) {
	switch msg := context.Message().(type) {
	case *SpawnUser:
		state.OnSpawnUser(context, msg)

	case *SpawnRoom:
		state.OnSpawnRoom(context, msg)

	case *DestroyedRoom:
		state.OnDestroyedRoom(context, msg)

	case *RoomList:
		state.OnRoomList(context, msg)

	default:
		fmt.Print(msg)
	}
}
