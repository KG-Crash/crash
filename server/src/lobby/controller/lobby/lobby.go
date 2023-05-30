package lobby

import (
	"encoding/json"
	"fmt"

	"github.com/KG-Crash/crash/lobby/service/http"
	"github.com/KG-Crash/crash/lobby/service/jwt"
	"github.com/KG-Crash/crash/lobby/setting"
	"github.com/KG-Crash/crash/protocol/request"
	"github.com/KG-Crash/crash/protocol/response"
	"github.com/gin-gonic/gin"
	"github.com/go-redis/redis"
	"github.com/google/uuid"
)

func createRoom(ctx *gin.Context) {
	setting := setting.Get()

	req := http.GetRequest[*request.RouteCreate](ctx)
	identity := req.Identity()
	if identity != request.ROUTE_CREATE {
		http.SetResponse(ctx, response.RouteEnter{Error: 1})
		return
	}

	roomID, err := uuid.NewUUID()
	if err != nil {
		http.SetResponse(ctx, response.CreateRoom{Error: 1})
		return
	}

	if err != nil {
		panic(err)
	}
	if err != nil {
		http.SetResponse(ctx, response.RouteCreate{Error: 1})
		return
	}

	index := 0 // 임시
	endpoint := setting.Server["game"][index]
	http.SetResponse(ctx, response.RouteCreate{Id: roomID.String(), Host: endpoint.Host, Port: uint32(endpoint.Port)})
}

func enterRoom(ctx *gin.Context) {
	setting := setting.Get()

	req := http.GetRequest[*request.RouteEnter](ctx)
	identity := req.Identity()

	if identity != request.ROUTE_ENTER {
		http.SetResponse(ctx, response.RouteEnter{Error: 1})
		return
	}

	index := 0 // 임시
	endpoint := setting.Server["game"][index]

	http.SetResponse(ctx, response.RouteEnter{
		Host: endpoint.Host,
		Port: uint32(endpoint.Port),
	})
}

func roomList(ctx *gin.Context) {
	setting := setting.Get()

	req := http.GetRequest[*request.RoomList](ctx)

	identity := req.Identity()

	if identity != request.ROOM_LIST {
		http.SetResponse(ctx, response.RoomList{Error: 1})
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

	http.SetResponse(ctx, res)
}

func Bind(r *gin.RouterGroup) {
	r.Use(jwt.Middleware)
	r.POST("/create-room", createRoom)
	r.POST("/enter-room", enterRoom)
	r.POST("/room", roomList)
}
