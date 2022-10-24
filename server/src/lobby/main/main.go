package main

import (
	"encoding/binary"
	"fmt"
	"game/protocol"
	"game/protocol/request"
	"game/protocol/response"
	"io/ioutil"
	"net/http"

	"github.com/gin-gonic/gin"
)

func Serialize(res protocol.Protocol) []byte {
	serialized := res.Serialize()
	size := uint32(len(serialized))

	bytes := make([]byte, 8, 8+size)
	binary.LittleEndian.PutUint32(bytes[:], uint32(4+len(serialized)))

	identity := uint32(res.Identity())
	binary.LittleEndian.PutUint32(bytes[4:], identity)

	bytes = append(bytes, serialized...)
	return bytes
}

func Deserialize(bytes []byte) protocol.Protocol {
	offset := 0
	size := binary.LittleEndian.Uint32(bytes[offset:4])
	offset += 4

	return request.Deserialize(size-4, bytes[offset:])
}

func main() {
	r := gin.Default()
	r.POST("/sample", func(c *gin.Context) {
		bytes, _ := ioutil.ReadAll(c.Request.Body)
		ptc := Deserialize(bytes)
		fmt.Println(ptc.Identity())

		c.Header("Content-Disposition", "attachment; filename=file-name.txt")
		c.Data(http.StatusOK, "application/octet-stream", Serialize(response.Login{
			Id: "test",
		}))
	})
	r.Run() // listen and serve on 0.0.0.0:8080 (for windows "localhost:8080")
}
