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

	"github.com/dgrijalva/jwt-go"

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

func Deserialize[T protocol.Protocol](bytes []byte) T {
	offset := 0
	size := binary.LittleEndian.Uint32(bytes[offset:4])
	offset += 4

	return request.Deserialize(size-4, bytes[offset:]).(T)
}

func GetRequest[T protocol.Protocol](c *gin.Context) T {
	bytes, _ := ioutil.ReadAll(c.Request.Body)
	return Deserialize[T](bytes)
}

func SetResponse[T protocol.Protocol](ctx *gin.Context, ptc T) {
	ctx.Data(http.StatusOK, "application/octet-stream", Serialize(ptc))
}

var ctx = context.Background()
var hmacSecret = []byte("kg-crash")

func main() {
	setting := setting.Get()

	// rdb := redis.NewClient(&redis.Options{
	// 	Addr:     fmt.Sprintf("%s:%d", setting.Redis.Host, setting.Redis.Port),
	// 	Password: "",                    // no password set
	// 	DB:       int(setting.Redis.Db), // use default DB
	// })

	// err := rdb.Set(ctx, "key", "value", 0).Err()
	// if err != nil {
	// 	panic(err)
	// }

	r := gin.Default()
	lobby := r.Group("/lobby")
	lobby.Use(func(ctx *gin.Context) {
		if ctx.Request.RequestURI == "/lobby/authentication" {
			ctx.Next()
			return
		}

		if len(ctx.Request.Header["Authorization"]) != 1 {
			SetResponse(ctx, response.HttpException{Error: 1})
			return
		}

		var tokenString string
		_, err := fmt.Sscanf("Bearer %s", tokenString)
		if err != nil {
			SetResponse(ctx, response.HttpException{Error: 1})
			return
		}

		token, err := jwt.Parse(tokenString, func(t *jwt.Token) (interface{}, error) {
			if _, ok := t.Method.(*jwt.SigningMethodHMAC); !ok {
				return nil, fmt.Errorf("Unexpected signing method: %v", t.Header["alg"])
			}

			return hmacSecret, nil
		})

		claims, ok := token.Claims.(jwt.MapClaims)
		if !ok || token.Valid {
			SetResponse(ctx, response.HttpException{Error: 1})
			return
		}

		id := claims["id"].(string)
		ctx.Request.Header["id"] = []string{id}
		ctx.Next()
	})

	r.POST("/authentication", func(ctx *gin.Context) {
		req := GetRequest[request.Authentication](ctx)
		identity := req.Identity()

		if identity != request.AUTHENTICATION {
			SetResponse(ctx, response.Authentication{Error: 1})
			return
		}

		token := jwt.NewWithClaims(jwt.SigningMethodHS256, jwt.MapClaims{
			"id": req.Id,
		})

		tokenString, err := token.SignedString(hmacSecret)
		if err != nil {
			SetResponse(ctx, response.Authentication{Error: 1})
			return
		}

		SetResponse(ctx, response.Authentication{Token: tokenString})
	})

	lobby.GET("/route", func(c *gin.Context) {
		req := GetRequest[request.Route](c)
		identity := req.Identity()

		if identity != request.CREATE_ROOM {
			c.Data(http.StatusOK, "application/octet-stream", Serialize(response.Route{
				Error: 1,
			}))
			return
		}

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
			Port: uint32(endpoint.Port),
		}))
	})

	lobby.GET("/room", func(ctx *gin.Context) {
		req := GetRequest[request.RoomList](ctx)

		identity := req.Identity()
		if identity != request.ROOM_LIST {
			SetResponse(ctx, response.RoomList{Error: 1})
			return
		}

		SetResponse(ctx, response.RoomList{Rooms: []response.Room{}})
	})
	r.Run() // listen and serve on 0.0.0.0:8080 (for windows "localhost:8080")
}
