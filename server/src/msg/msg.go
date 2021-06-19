package msg

import (
	"net"
	"protocol"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type OnAccepted func(context actor.Context, conn net.Conn)
type OnReceived func(context actor.Context, id string, protocol protocol.Protocol)

type Listen struct {
	Port uint16
}

type Accept struct {
}

type Accepted struct {
	net.Conn
}

type BindAcceptor struct {
	OnAccepted
}

type SetConnection struct {
	net.Conn
}

type Receive struct {
}

type Disconnected struct {
	*actor.PID
}

type Write struct {
	protocol.Protocol
}

type Received struct {
	protocol.Protocol
}

type SpawnUser struct {
	net.Conn
	OnReceived
}

type SpawnRoom struct {
	Master *actor.PID
	UserId string
}

type RoomList struct {
	User *actor.PID
}

type BindUser struct {
	Id string
	OnReceived
}

type JoinedRoom struct {
	UserId string
	User   *actor.PID
	Room   *actor.PID
	Master bool
	Users  []string
	Error  uint32
}

type LeaveRoom struct {
	UserId string
	User   *actor.PID
}

type Logout struct {
	UserId string
}

type Chat struct {
	UserId  string
	Message string
}

type Whisper struct {
	From    string
	To      string
	Message string
}

type Kick struct {
	From string
	To   string
}

type Kicked struct {
}

type JoinRoom struct {
	User   *actor.PID
	UserId string
	RoomId string
}

type LeavedRoom struct {
	User   *actor.PID
	UserId string
	Error  uint32
}

type DestroyedRoom struct {
	RoomId string
}
