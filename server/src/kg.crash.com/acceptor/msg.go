package acceptor

import "net"

type accept struct{}

type Connected struct {
	Conn net.Conn
}
