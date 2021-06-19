package main

import (
	"fmt"
	"log"
	"model"
	"msg"
	"net"
	"network"
	"os"
	"protocol"
	"protocol/request"
	"strconv"

	console "github.com/AsynkronIT/goconsole"
	"github.com/AsynkronIT/protoactor-go/actor"
	"gorm.io/driver/mysql"
	"gorm.io/gorm"
)

var game *actor.PID

func OnReceived(context actor.Context, id string, protocol protocol.Protocol) {
	switch x := protocol.(type) {
	case *request.CreateRoom:
		context.Send(game, &msg.SpawnRoom{
			Master: context.Self(),
			UserId: id,
		})

	case *request.JoinRoom:
		context.Send(game, &msg.JoinRoom{
			User:   context.Self(),
			UserId: id,
			RoomId: x.Id,
		})

	case *request.LeaveRoom: // game > user > room
		context.Send(context.Self(), &msg.LeaveRoom{
			UserId: id,
			User:   context.Self(),
		})

	case *request.RoomList:
		context.Send(game, &msg.RoomList{
			User: context.Self(),
		})

	case *request.Chat:
		context.Send(context.Self(), &msg.Chat{
			UserId:  id,
			Message: x.Message,
		})

	case *request.Whisper:
		context.Send(game, &msg.Whisper{
			From:    id,
			To:      x.User,
			Message: x.Message,
		})

	case *request.KickRoom:
		context.Send(context.Self(), &msg.Kick{
			From: id,
			To:   x.User,
		})
	}
}

func OnAccepted(context actor.Context, conn net.Conn) {
	context.Send(game, &msg.SpawnUser{
		Conn:       conn,
		OnReceived: OnReceived,
	})
}

type User struct {
	Id   uint64 `gorm:"primaryKey;autoIncrement:true"`
	Name string `gorm:"index;default:default name"`
}

type Room struct {
	UserId uint64 `gorm:"index"`
	User   User   `gorm:"foreignKey:Id;references:UserId"`
}

func main() {
	dsn := "crash:kg_crash@tcp(127.0.0.1:3306)/crash?charset=utf8mb4&parseTime=True&loc=Local"
	db, err := gorm.Open(mysql.Open(dsn), &gorm.Config{})
	fmt.Println(db)

	db.AutoMigrate(&User{})
	db.AutoMigrate(&Room{})

	x := Room{}
	db.Joins("User").Where(&Room{UserId: 4}).First(&x)
	fmt.Println(x)

	// user := &User{}
	// db.Create(user)
	// room := &Room{
	// 	User: *user,
	// }
	// db.Create(room)

	log.SetFlags(log.Ldate | log.Ltime)

	system := actor.NewActorSystem()

	game = system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return model.NewGame()
	}))

	acceptor := system.Root.Spawn(actor.PropsFromProducer(func() actor.Actor {
		return &network.AcceptorActor{}
	}))
	system.Root.Send(acceptor, &msg.BindAcceptor{
		OnAccepted: OnAccepted,
	})

	port := "8000"
	args := os.Args[1:]
	if len(args) > 0 {
		port = args[0]
	}

	p, err := strconv.Atoi(port)
	if err != nil {
		log.Fatalf("Port must be unsigned short type. %s does not port format.", port)
		return
	}

	system.Root.Send(acceptor, &msg.Listen{Port: uint16(p)})
	_, _ = console.ReadLine()
}
