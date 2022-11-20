package main

import (
	"context"
	"encoding/binary"
	"io/ioutil"
	"net/http"
	"protocol"
	"protocol/request"
	"protocol/response"

	"github.com/go-redis/redis"

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

var ctx = context.Background()

func main() {
	rdb := redis.NewClient(&redis.Options{
		Addr:     "localhost:6379",
		Password: "", // no password set
		DB:       0,  // use default DB
	})

	err := rdb.Set(ctx, "key", "value", 0).Err()
	if err != nil {
		panic(err)
	}

	r := gin.Default()

	r.GET("/room", func(c *gin.Context) {
		bytes, _ := ioutil.ReadAll(c.Request.Body)
		ptc := Deserialize(bytes)

		identity := ptc.Identity()
		if identity != request.ROOM_LIST {
			c.Data(http.StatusOK, "application/octet-stream", Serialize(response.RoomList{
				Error: 1,
			}))
			return
		}

		c.Data(http.StatusOK, "application/octet-stream", Serialize(response.RoomList{
			Rooms: []response.Room{},
		}))
	})
	r.Run() // listen and serve on 0.0.0.0:8080 (for windows "localhost:8080")
}
