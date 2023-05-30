package http

import (
	"encoding/binary"
	"io/ioutil"
	"net/http"

	"github.com/KG-Crash/crash/protocol"
	"github.com/KG-Crash/crash/protocol/request"
	"github.com/gin-gonic/gin"
)

func serialize(res protocol.Protocol) []byte {
	serialized := res.Serialize()
	size := uint32(len(serialized))

	bytes := make([]byte, 8, 8+size)
	binary.LittleEndian.PutUint32(bytes[:], uint32(4+len(serialized)))

	identity := uint32(res.Identity())
	binary.LittleEndian.PutUint32(bytes[4:], identity)

	bytes = append(bytes, serialized...)
	return bytes
}

func deserialize[T protocol.Protocol](bytes []byte) T {
	offset := 0
	size := binary.LittleEndian.Uint32(bytes[offset:4])
	offset += 4

	return request.Deserialize(size-4, bytes[offset:]).(T)
}

func GetRequest[T protocol.Protocol](c *gin.Context) T {
	bytes, _ := ioutil.ReadAll(c.Request.Body)
	return deserialize[T](bytes)
}

func SetResponse[T protocol.Protocol](ctx *gin.Context, ptc T) {
	ctx.Data(http.StatusOK, "application/octet-stream", serialize(ptc))
}
