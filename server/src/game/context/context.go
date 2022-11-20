package context

import (
	"crypto/rand"
	"errors"
	"math"
	"math/big"
	"model"
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
		if value.Playing {
			continue
		}

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

func (ctx *Context) Leave(session *model.Session) (*model.Session, error) {
	room := session.Room
	if room == nil {
		return nil, errors.New("")
	}

	team, err := session.GetTeam()
	if err != nil {
		return nil, errors.New("")
	}

	var master *model.Session = nil
	users := room.GetAllUsers()
	if len(users) > 1 {
		master = room.NextMaster()
		room.Master = master
	}

	delete(room.Users[team], session.ID())
	if len(room.Users[team]) == 0 {
		delete(room.Users, team)
	}

	if len(room.Users) == 0 {
		delete(ctx.Rooms, room.ID())
	}

	session.Room = nil
	return master, nil
}

func (ctx *Context) OnLeaveRoom(session *model.Session, req request.LeaveRoom) {

	room := session.Room
	if room == nil {
		session.Send(response.LeaveRoom{
			Error: 1,
		})
		return
	}

	users := room.GetAllUsers()

	master, err := ctx.Leave(session)
	if err != nil {
		session.Send(response.LeaveRoom{
			Error: 1,
		})
		return
	}

	res := response.LeaveRoom{
		User: session.ID(),
	}
	if master != nil {
		res.NewMaster = master.ID()
	}

	for _, user := range users {
		user.Send(res)
	}
}

func (ctx *Context) OnChat(session *model.Session, req request.Chat) {

	if session.Room == nil {
		session.Send(response.Chat{
			Error: 1,
		})
		return
	}

	for _, user := range session.Room.GetAllUsers() {
		user.Send(response.Chat{
			User:    session.ID(),
			Message: req.Message,
		})
	}
}

func (ctx *Context) OnGameChat(session *model.Session, req request.InGameChat) {
	room := session.Room
	if room == nil {
		session.Send(response.InGameChat{
			Error: 1,
		})
		return
	}

	for _, user := range session.Room.GetAllUsers() {
		user.Send(response.InGameChat{
			Turn:     req.Frame,
			Frame:    req.Frame,
			Message:  req.Message,
			Sequence: int32(room.Sequences[session]),
		})
	}
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
	session.Room.Playing = true
	for _, user := range session.Room.GetAllUsers() {
		user.Send(response.GameStart{
			Seed: session.Room.Seed,
		})
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

	for ready := range room.Sequences {
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
		Sequence: int32(seq),
		Actions:  []response.Action{},
		Turn:     req.Turn,
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

func (ctx *Context) OnWhisper(session *model.Session, req request.Whisper) {

	to, ok := ctx.Sessions[req.User]
	if !ok {
		session.Send(response.Whisper{
			Error: 1,
		})
		return
	}

	res := response.Whisper{
		From:    session.ID(),
		To:      to.ID(),
		Message: req.Message,
	}
	session.Send(res)
	to.Send(res)
}

func (ctx *Context) OnKick(session *model.Session, req request.KickRoom) {

	if session.Room == nil {
		session.Send(response.KickRoom{
			Error: 1,
		})
		return
	}

	if session.Room.Master != session {
		session.Send(response.KickRoom{
			Error: 1,
		})
		return
	}

	target, ok := ctx.Sessions[req.User]
	if !ok {
		session.Send(response.KickRoom{
			Error: 1,
		})
	}

	users := session.Room.GetAllUsers()

	_, err := ctx.Leave(target)
	if err != nil {
		session.Send(response.KickRoom{
			Error: 1,
		})
	}

	for _, user := range users {
		if user == session {
			user.Send(response.KickRoom{})
		} else if user == target {
			user.Send(response.KickedRoom{})
		} else {
			user.Send(response.LeaveRoom{
				User: target.ID(),
			})
		}
	}
}

func (ctx *Context) OnExit(session *model.Session) {
	room := session.Room
	if room != nil {
		users := room.GetAllUsers()

		master, _ := ctx.Leave(session)
		res := response.LeaveRoom{
			User: session.ID(),
		}
		if master != nil {
			res.NewMaster = master.ID()
		}
		for _, user := range users {
			if user == session {
				continue
			}

			user.Send(res)
		}
	}

	delete(ctx.Sessions, session.ID())
}
