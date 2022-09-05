package context

import (
	"KG/model"
	"crypto/rand"
	"math"
	"math/big"
	"protocol/request"
	"protocol/response"
)

type Context struct {
	Sessions map[string]*model.Session
	Rooms    map[string]*model.Room
}

func New() *Context {
	return &Context{
		Sessions: map[string]*model.Session{},
		Rooms:    map[string]*model.Room{},
	}
}

func (ctx *Context) OnRoomList(session *model.Session, req request.RoomList) {

	rooms := []response.Room{}

	for _, value := range ctx.Rooms {
		rooms = append(rooms, response.Room{
			Id:    value.ID(),
			Title: value.Title(),
		})
	}

	session.Send(response.RoomList{
		Rooms: rooms,
	})
}

func (ctx *Context) OnCreateRoom(session *model.Session, req request.CreateRoom) {
	if session.Room != nil {
		session.Send(response.CreateRoom{
			Error: 1,
		})
		return
	}

	room := model.NewRoom(session, model.RoomConfig{
		Team:  req.Teams,
		Title: req.Title,
	})

	ctx.Rooms[room.ID()] = &room
	session.Send(response.CreateRoom{
		Id: room.ID(),
	})
}

func (ctx *Context) OnEnterRoom(session *model.Session, req request.EnterRoom) {
	if session.Room != nil {
		session.Send(response.EnterRoom{
			Error: 1,
		})
		return
	}

	room, ok := ctx.Rooms[req.Id]
	if !ok {
		session.Send(response.EnterRoom{
			Error: 1,
		})
		return
	}

	if err := room.Enter(session); err != nil {
		session.Send(response.EnterRoom{
			Error: 1,
		})
		return
	}

	res := response.EnterRoom{
		User:   session.ID(),
		Users:  []response.User{},
		Master: room.Master.ID(),
		Error:  0,
	}

	for team, users := range room.Users {
		for _, user := range users {
			user.Room = room

			res.Users = append(res.Users, response.User{
				Id:       user.ID(),
				Team:     int32(team),
				Sequence: -1,
			})
		}
	}

	for _, user := range room.GetAllUsers() {
		user.Send(res)
	}
}

func (ctx *Context) OnChat(session *model.Session, req request.Chat) {

}

func (ctx *Context) OnGameStart(session *model.Session, req request.GameStart) {
	if session.Room == nil {
		session.Send(response.GameStart{
			Error: 1,
		})
		return
	}

	if session.Room.Master != session {
		session.Send(response.GameStart{
			Error: 1,
		})
		return
	}

	if !session.Room.IsPlayable() {
		session.Send(response.GameStart{
			Error: 1,
		})
		return
	}

	seed, _ := rand.Int(rand.Reader, big.NewInt(math.MaxInt64))
	session.Room.Seed = seed.Int64()

	// TODO: Ready에서 seed 주지말고
	// 여기서 전달해도 될 것 같음
	for _, user := range session.Room.GetAllUsers() {
		user.Send(response.GameStart{})
	}
}

func (ctx *Context) OnReady(session *model.Session, req request.Ready) {

	room := session.Room
	if room == nil {
		session.Send(response.Ready{
			Error: 1,
		})
	}

	next := len(room.Sequences)
	room.Sequences[session] = next

	res := response.Ready{
		Seed:  room.Seed,
		Users: []response.User{},
	}

	for team, users := range room.Users {
		for _, user := range users {
			seq, ok := room.Sequences[user]
			if !ok {
				seq = -1
			}

			res.Users = append(res.Users, response.User{
				Id:       user.ID(),
				Team:     int32(team),
				Sequence: int32(seq),
			})
		}
	}

	for ready, _ := range room.Sequences {
		ready.Send(res)
	}
}

func (ctx *Context) OnAction(session *model.Session, req request.ActionQueue) {
	room := session.Room
	if room == nil {
		session.Send(response.ActionQueue{
			Error: 1,
		})
	}

	seq, ok := room.Sequences[session]
	if !ok {
		session.Send(response.ActionQueue{
			Error: 1,
		})
	}
	res := response.ActionQueue{
		User:    int32(seq),
		Actions: []response.Action{},
		Turn:    req.Turn,
	}

	for _, action := range req.Actions {
		res.Actions = append(res.Actions, response.Action{
			Frame:  action.Frame,
			Id:     action.Id,
			Param1: action.Param1,
			Param2: action.Param2,
		})
	}

	for _, user := range room.GetAllUsers() {
		user.Send(res)
	}
}
