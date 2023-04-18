package main

import (
	"context"
	"encoding/binary"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"

	"github.com/KG-Crash/crash/lobby/setting"
	"github.com/KG-Crash/crash/protocol"
	"github.com/KG-Crash/crash/protocol/request"
	"github.com/KG-Crash/crash/protocol/response"

	"github.com/dgrijalva/jwt-go"
	"github.com/go-redis/redis"
	"github.com/google/uuid"

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
		_, err := fmt.Sscanf(ctx.Request.Header["Authorization"][0], "Bearer %s", &tokenString)
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
		if !ok || !token.Valid {
			SetResponse(ctx, response.HttpException{Error: 1})
			return
		}

		id := claims["id"].(string)
		ctx.Request.Header["id"] = []string{id}
		ctx.Next()
	})

	r.POST("/authentication", func(ctx *gin.Context) {
		req := GetRequest[*request.Authentication](ctx)
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

	lobby.POST("/create-room", func(ctx *gin.Context) {
		req := GetRequest[*request.RouteCreate](ctx)
		identity := req.Identity()
		if identity != request.ROUTE_CREATE {
			SetResponse(ctx, response.RouteEnter{Error: 1})
			return
		}

		roomID, err := uuid.NewUUID()
		if err != nil {
			SetResponse(ctx, response.CreateRoom{Error: 1})
			return
		}

		// rdb := redis.NewClient(&redis.Options{
		// 	Addr:     fmt.Sprintf("%s:%d", setting.Redis.Host, setting.Redis.Port),
		// 	Password: "",
		// 	DB:       int(setting.Redis.Db),
		// })

		// err = rdb.Set(ctx, "key", "value", 0).Err()
		if err != nil {
			panic(err)
		}
		if err != nil {
			SetResponse(ctx, response.RouteCreate{Error: 1})
			return
		}

		index := 0 // 임시
		endpoint := setting.Server["game"][index]
		SetResponse(ctx, response.RouteCreate{Id: roomID.String(), Host: endpoint.Host, Port: uint32(endpoint.Port)})
	})

	lobby.POST("/enter-room", func(ctx *gin.Context) {
		req := GetRequest[*request.RouteEnter](ctx)
		identity := req.Identity()

		if identity != request.ROUTE_ENTER {
			SetResponse(ctx, response.RouteEnter{Error: 1})
			return
		}

		index := 0 // 임시
		endpoint := setting.Server["game"][index]

		SetResponse(ctx, response.RouteEnter{
			Host: endpoint.Host,
			Port: uint32(endpoint.Port),
		})
	})

	lobby.POST("/room", func(ctx *gin.Context) {
		req := GetRequest[*request.RoomList](ctx)

		identity := req.Identity()

		if identity != request.ROOM_LIST {
			SetResponse(ctx, response.RoomList{Error: 1})
			return
		}

		res := response.RoomList{
			Rooms: []response.Room{},
		}
		for _, redisSetting := range setting.Redis {
			rdb := redis.NewClient(&redis.Options{
				Addr: fmt.Sprintf("%s:%d", redisSetting.Host, redisSetting.Port),
				DB:   int(redisSetting.Db),
			})

			redisResult, _ := rdb.HGetAll("room").Result()
			for _, stringify := range redisResult {
				var room response.Room
				json.Unmarshal([]byte(stringify), &room)
				res.Rooms = append(res.Rooms, room)
			}
		}

		SetResponse(ctx, res)
	})
	r.Run(":8001")
}
