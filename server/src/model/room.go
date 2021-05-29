package model

import (
	"github.com/AsynkronIT/protoactor-go/actor"
)

type RoomActor struct {
	Id     string
	Users  []*actor.PID
	Master *actor.PID
}

type Join struct {
}

func NewRoom(id string, master *actor.PID) *RoomActor {
	return &RoomActor{
		Id:     id,
		Users:  make([]*actor.PID, 0),
		Master: master,
	}
}

func (state *RoomActor) Receive(context actor.Context) {
	switch context.Message().(type) {
	case *Join:

	}
}
