package room

import (
	"KG/session"
	"crypto/rand"
	"errors"
	"math"
	"math/big"

	"github.com/google/uuid"
)

type Config struct {
	Team  []int32
	Title string
}

type Room struct {
	Id      string
	Users   map[int][]*session.Session
	Playing bool
	Config  Config
	Master  *session.Session
	Seed    int64
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

func (room *Room) Enter(session *session.Session) error {

	team, err := room.nextTeamSequence()
	if err != nil {
		return err
	}

	room.Users[team] = append(room.Users[team], session)
	return nil
}

func New(master *session.Session, config Config) Room {
	seed, _ := rand.Int(rand.Reader, big.NewInt(math.MaxInt64))
	room := Room{
		Id:      uuid.NewString(),
		Users:   map[int][]*session.Session{},
		Playing: false,
		Config:  config,
		Master:  master,
		Seed:    seed.Int64(),
	}

	room.Enter(master)
	return room
}
