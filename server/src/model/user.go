package model

import "network"

type User struct {
	session *network.Session
	room    *Room
}

func NewUser(session *network.Session) *User {
	return &User{
		session: session,
		room:    nil,
	}
}
