package receiver

import "protocol"

type receive struct{}

type Received struct {
	protocol.Protocol
}

type Disconnected struct{}
