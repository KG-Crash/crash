package model

import (
	"sync/atomic"
)

type RoomEnter struct {
	*User
}
type RoomLeave struct {
	*User
}

type Room struct {
	id              uint32
	users           map[string]*User
	master          *User
	messages        chan interface{}
	callbackDestroy func(*Room)
}

var index uint32 // TODO 스케일아웃하면 Redis에서 관리할 필요가 있음

func init() {
	index = 0
}

func NewRoom(master *User, callbackDestroy func(*Room)) *Room {
	room := &Room{
		id:              index,
		master:          master,
		users:           make(map[string]*User),
		callbackDestroy: callbackDestroy,
	}
	atomic.AddUint32(&index, 1)

	return room
}

func (room *Room) process() {

	for {
		message := <-room.messages
		switch msg := message.(type) {
		case *RoomEnter:
			if room.contains(msg.User) {
				return
			}

			room.users[msg.User.session.Host()] = msg.User

		case *RoomLeave:
			if !room.contains(msg.User) {
				return
			}

			if room.master == msg.User {
				room.callbackDestroy(room)
			} else {
				delete(room.users, msg.User.session.Host())
			}
		}
	}
}

func (room *Room) contains(user *User) bool {
	_, ok := room.users[user.session.Host()]
	return ok
}

func (room *Room) Enter(user *User) {
	room.messages <- &RoomEnter{User: user}
}

func (room *Room) Leave(user *User) {
	room.messages <- &RoomLeave{User: user}
}
