package handler

import (
	"fmt"
	"game/protocol"
)

type Session interface {
}

type Message struct {
	Session
	protocol.Protocol
}

type Handler struct {
	funcs       map[int]func(Session, interface{})
	exitHandler func(Session)
	Channel     chan Message
	Exit        chan Session
}

func New() *Handler {

	created := &Handler{
		funcs:       map[int]func(Session, interface{}){},
		exitHandler: nil,
		Channel:     make(chan Message),
		Exit:        make(chan Session),
	}

	go created.Loop()
	return created
}

func Register[S Session, T protocol.Protocol](handler *Handler, fn func(session *S, packet T)) {

	var template T
	id := template.Identity()

	handler.funcs[id] = func(session Session, ptc interface{}) {
		fn(session.(*S), *ptc.(*T))
	}
}

func RegisterExit[S Session](handler *Handler, fn func(session *S)) {
	handler.exitHandler = func(session Session) {
		fn(session.(*S))
	}
}

func (handler *Handler) Invoke(session Session, ptc protocol.Protocol) {
	id := ptc.Identity()
	if fn, ok := handler.funcs[id]; ok {
		fn(session, ptc)
	} else {
		fmt.Printf("invalid command %d", id)
	}
}

func (handler *Handler) Loop() {
	for {
		select {
		case message := <-handler.Channel:
			handler.Invoke(message.Session, message.Protocol)

		case session := <-handler.Exit:
			if handler.exitHandler != nil {
				handler.exitHandler(session)
			}
		}
	}
}
