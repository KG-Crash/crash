package Protocol

type Protocol interface {
	Serialize() []byte
	Identity() int
}
