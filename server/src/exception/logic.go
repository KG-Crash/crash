package exception

type Error interface {
	Code() uint32
}

type exception struct {
	code uint32
}

func New(code uint32) *exception {
	return &exception{
		code: code,
	}
}

func (e *exception) Code() uint32 {
	return e.code
}
