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
