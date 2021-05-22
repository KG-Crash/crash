package model

import (
	"sync"
)

type Room struct {
	id     uint32
	users  map[string]*User
	master *User

	in  chan *User
	out chan *User

	callbackDestroy func(*Room)
}

var index uint32 // TODO 스케일아웃하면 Redis에서 관리할 필요가 있음
var mutex *sync.Mutex

func init() {
	index = 0
	mutex = &sync.Mutex{}
}

func NewRoom(master *User, callbackDestroy func(*Room)) *Room {
	mutex.Lock()
	room := &Room{
		id:              index,
		master:          master,
		users:           make(map[string]*User),
		callbackDestroy: callbackDestroy,
	}
	index++
	mutex.Unlock()

	return room
}

func (room *Room) process() {

	for {
		select {
		case user := <-room.in:
			if room.contains(user) {
				return
			}

			room.users[user.session.Host()] = user

		case user := <-room.out:
			if !room.contains(user) {
				return
			}

			if room.master == user {
				room.callbackDestroy(room)
			} else {
				delete(room.users, user.session.Host())
			}
		}
	}
}

func (room *Room) contains(user *User) bool {
	_, ok := room.users[user.session.Host()]
	return ok
}

func (room *Room) Enter(user *User) {
	room.in <- user
}

func (room *Room) Leave(user *User) {
	room.out <- user
}

func (room *Room) Start() {

}
