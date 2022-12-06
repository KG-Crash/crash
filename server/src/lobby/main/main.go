package main

import (
	"context"
	"encoding/binary"
	"fmt"
	"io/ioutil"
	"lobby/setting"
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
	setting := setting.Get()

	rdb := redis.NewClient(&redis.Options{
		Addr:     fmt.Sprintf("%s:%d", setting.Redis.Host, setting.Redis.Port),
		Password: "",                    // no password set
		DB:       int(setting.Redis.Db), // use default DB
	})

	err := rdb.Set(ctx, "key", "value", 0).Err()
	if err != nil {
		panic(err)
	}

	r := gin.Default()

	r.GET("/route", func(c *gin.Context) {
		bytes, _ := ioutil.ReadAll(c.Request.Body)
		ptc := Deserialize(bytes)
		identity := ptc.Identity()

		if identity != request.CREATE_ROOM {
			c.Data(http.StatusOK, "application/octet-stream", Serialize(response.Route{
				Error: 1,
			}))
			return
		}

		req := ptc.(request.Route)
		if req.Value == "" { // req.Value는 room id로 해야할듯
			c.Data(http.StatusOK, "application/octet-stream", Serialize(response.Route{
				Error: 2,
			}))
			return
		}

		index := 0 // 임시
		endpoint := setting.Server["game"][index]

		c.Data(http.StatusOK, "application/octet-stream", Serialize(response.Route{
			Host: endpoint.Host,
			Port: endpoint.Port,
		}))
	})

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
