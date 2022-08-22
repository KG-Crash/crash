package context

import (
	"KG/room"
	"KG/session"
	"protocol/request"
	"protocol/response"
)

type Context struct {
	Sessions map[string]*session.Session
	Rooms    map[string]*room.Room
}

func New() *Context {
	return &Context{
		Sessions: map[string]*session.Session{},
		Rooms:    map[string]*room.Room{},
	}
}

func (ctx *Context) OnRoomList(session *session.Session, req request.RoomList) {

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

func (ctx *Context) OnCreateRoom(session *session.Session, req request.CreateRoom) {
	room := room.New(session, room.Config{
		Team:  req.Teams,
		Title: req.Title,
	})

	ctx.Rooms[room.ID()] = &room
	session.Send(response.CreateRoom{
		Id: room.ID(),
	})
}

func (ctx *Context) OnEnterRoom(session *session.Session, req request.EnterRoom) {
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
			res.Users = append(res.Users, response.User{
				Id:       user.ID(),
				Team:     int32(team),
				Sequence: -1,
			})
		}
	}

	session.Send(res)
}
