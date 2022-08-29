package model

import (
	"KG/handler"
	"encoding/binary"
	"io"
	"net"
	"protocol"
	"protocol/request"

	"crypto/rand"
	"errors"
	"math"
	"math/big"

	"github.com/google/uuid"
)

type Session struct {
	net.Conn
	queue   []byte
	handler *handler.Handler
	id      string
	*Room
}

type RoomConfig struct {
	Team  []int32
	Title string
}

type Room struct {
	Id        string
	Users     map[int][]*Session
	Playing   bool
	Config    RoomConfig
	Master    *Session
	Seed      int64
	Sequences map[int]*Session
}

func NewSession(conn net.Conn, handler *handler.Handler) *Session {
	return &Session{
		Conn:    conn,
		queue:   make([]byte, 0, 4096),
		handler: handler,
		id:      uuid.NewString(),
	}
}

func (session *Session) ID() string {
	return session.id
}

func (session *Session) Send(res protocol.Protocol) {
	serialized := res.Serialize()
	size := uint32(len(serialized))

	bytes := make([]byte, 8, 8+size)
	binary.LittleEndian.PutUint32(bytes[:], uint32(4+len(serialized)))

	identity := uint32(res.Identity())
	binary.LittleEndian.PutUint32(bytes[4:], identity)

	bytes = append(bytes, serialized...)
	session.Conn.Write(bytes)
}

func (session *Session) GetTeam() (int, error) {
	if session.Room == nil {
		return 0, errors.New("session does not enter game room")
	}

	for team, users := range session.Room.Users {
		for _, user := range users {
			if user == session {
				return team, nil
			}
		}
	}

	return 0, errors.New("session does not contains any team")
}

func (session *Session) Loop() {
	for {
		buffer := make([]byte, 4096)
		numRead, err := session.Conn.Read(buffer)
		if numRead == 0 || err == io.EOF {
			// disconnected
			break
		}

		session.queue = append(session.queue, buffer[:numRead]...)
		for len(session.queue) > 0 {
			offset := 0
			size := binary.LittleEndian.Uint32(session.queue[offset:4])
			offset += 4

			if uint32(len(session.queue[offset:])) < size {
				break
			}

			deserialized := request.Deserialize(size-4, session.queue[offset:])
			session.handler.Channel <- handler.Message{
				Session:  session,
				Protocol: deserialized}
			session.queue = session.queue[size+4:]
		}
	}
}

func (room *Room) ID() string {
	return room.Id
}

func (room *Room) Title() string {
	return room.Config.Title
}

func (room *Room) nextTeamSequence() (int, error) {
	min := 0xFFFFFFFF
	result := -1
	for team, max := range room.Config.Team {
		users, ok := room.Users[team]
		count := 0
		if ok {
			count = len(users)
		}

		isFull := int(max) <= count
		if isFull {
			continue
		}

		if count >= min {
			continue
		}

		result = team
	}

	if result == -1 {
		return result, errors.New("full")
	}

	return result, nil
}

func (room *Room) Enter(session *Session) error {

	team, err := room.nextTeamSequence()
	if err != nil {
		return err
	}

	room.Users[team] = append(room.Users[team], session)
	return nil
}

func NewRoom(master *Session, config RoomConfig) Room {
	seed, _ := rand.Int(rand.Reader, big.NewInt(math.MaxInt64))
	room := Room{
		Id:      uuid.NewString(),
		Users:   map[int][]*Session{},
		Playing: false,
		Config:  config,
		Master:  master,
		Seed:    seed.Int64(),
	}

	room.Enter(master)
	return room
}

func (room *Room) GetAllUsers() []*Session {
	users := []*Session{}

	for _, users := range room.Users {
		for _, user := range users {
			users = append(users, user)
		}
	}

	return users
}

func (room *Room) IsPlayable() bool {

	// 0명이 되는 순간 팀 자체가 제거된다는 전제 하에 작동하는 코드
	// OnLeave에서 반드시 처리해줄것
	numTeam := len(room.Users)
	if numTeam < 2 {
		return false
	}

	return true
}
