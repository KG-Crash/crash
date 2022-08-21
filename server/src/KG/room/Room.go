package room

import (
	"KG/session"
	"crypto/rand"
	"math"
	"math/big"

	"github.com/google/uuid"
)

type Config struct {
	Team  []int32
	Title string
}

type Room struct {
	id           string
	users        map[int][]*session.Session
	playing      bool
	config       Config
	master       *session.Session
	seed         int64
	nextSequence int32
	mapSequence  map[int32]*session.Session
}

func (room *Room) ID() string {
	return room.id
}

func (room *Room) Title() string {
	return room.config.Title
}

func (room *Room) emptyTeamID() (int, error) {

	return 0, nil
}

func New(master *session.Session, config Config) Room {
	seed, _ := rand.Int(rand.Reader, big.NewInt(math.MaxInt64))
	return Room{
		id:           uuid.NewString(),
		users:        map[int][]*session.Session{},
		playing:      false,
		config:       config,
		master:       master,
		seed:         seed.Int64(),
		nextSequence: 0,
		mapSequence:  map[int32]*session.Session{},
	}
}
