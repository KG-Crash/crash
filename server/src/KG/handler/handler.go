package handler

import (
	"protocol"
)

type Session interface {
}

type Message struct {
	Session
	protocol.Protocol
}

type Handler struct {
	funcs   map[int]func(Session, interface{})
	Channel chan Message
}

func New() *Handler {

	created := &Handler{
		funcs:   map[int]func(Session, interface{}){},
		Channel: make(chan Message),
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

func (handler *Handler) Invoke(session Session, ptc protocol.Protocol) {
	id := ptc.Identity()
	if fn, ok := handler.funcs[id]; ok {
		fn(session, ptc)
	}
}

func (handler *Handler) Loop() {
	for {
		message := <-handler.Channel
		handler.Invoke(message.Session, message.Protocol)
	}
}
