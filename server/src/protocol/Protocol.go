package protocol

type Protocol interface {
	Serialize() []byte
	Deserialize(bytes []byte) Protocol
	Identity() int
}
