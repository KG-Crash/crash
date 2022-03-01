package user

import (
	"enum"
	"log"
	"msg"
	"net"
	"protocol"
	"protocol/request"
	"protocol/response"
	"time"

	"github.com/AsynkronIT/protoactor-go/actor"
	"kg.crash.com/session"
	"kg.crash.com/session/receiver"
	"kg.crash.com/session/sender"
)

type Actor struct {
	conn    net.Conn
	session *actor.PID

	ID   string
	Room *actor.PID
}

func New(id string, conn net.Conn) *Actor {
	return &Actor{
		conn: conn,
		ID:   id,
	}
}

func toUserResponse(user msg.UserState) response.User {
	return response.User{
		Id:   user.ID,
		Team: int32(user.Team),
	}
}

func toUserResponses(users []msg.UserState) []response.User {
	result := []response.User{}

	for _, user := range users {
		result = append(result, toUserResponse(user))
	}

	return result
}

func (state *Actor) onReceiveFlatBuffer(ctx actor.Context, p protocol.Protocol) {

	switch ptc := p.(type) {
	case *request.EnterRoom:
		if state.Room != nil {
			ctx.Send(ctx.Self(), &response.EnterRoom{
				Error: enum.ResultCode.AlreadyEnteredGameRoom,
			})
			return
		}

		getRoomFuture := ctx.RequestFuture(ctx.Parent(), &msg.RequestGetRoom{
			ID: ptc.Id,
		}, time.Hour)
		ctx.AwaitFuture(getRoomFuture, func(res interface{}, err error) {
			getRoom := res.(*msg.ResponseGetRoom)
			enterRoomFuture := ctx.RequestFuture(getRoom.Room, &msg.RequestEnterRoom{
				Sender: ctx.Self(),
			}, time.Hour)
			ctx.AwaitFuture(enterRoomFuture, func(res interface{}, err error) {
				enterRoom := res.(*msg.ResponseEnterRoom)

				result := &response.EnterRoom{
					User:   state.ID,
					Users:  toUserResponses(enterRoom.Users),
					Master: enterRoom.Master.ID,
				}

				for _, user := range enterRoom.Users {
					ctx.Send(user.PID, result)
				}

				state.Room = getRoom.Room
			})
		})

	case *request.RoomList:
		roomListFuture := ctx.RequestFuture(ctx.Parent(), &msg.RequestGetRoomList{}, time.Hour)
		ctx.AwaitFuture(roomListFuture, func(res interface{}, err error) {

			roomList := res.(*msg.ResponseGetRoomList)
			result := &response.RoomList{
				Rooms: []response.Room{},
			}

			count := roomList.Rooms.Len()
			if count > 0 {
				roomList.Rooms.ForEach(func(i int, pid *actor.PID) {
					roomStateFuture := ctx.RequestFuture(pid, &msg.RequestGetRoomState{}, time.Hour)
					ctx.AwaitFuture(roomStateFuture, func(res interface{}, err error) {

						roomState := res.(*msg.ResponseGetRoomState)
						result.Rooms = append(result.Rooms, response.Room{
							Id:    roomState.State.ID,
							Title: roomState.State.Title,
						})

						if len(result.Rooms) == count {
							ctx.Send(ctx.Self(), result)
						}
					})
				})
			} else {
				ctx.Send(ctx.Self(), result)
			}
		})

	case *request.CreateRoom:
		if state.Room != nil {
			ctx.Send(ctx.Self(), &response.CreateRoom{
				Error: enum.ResultCode.AlreadyEnteredGameRoom,
			})
			return
		}

		if len(ptc.Teams) < 2 {
			ctx.Send(ctx.Self(), &response.CreateRoom{
				Error: enum.ResultCode.NotEnoughTeams, // 팀은 2개 이상 설정되어야 함
			})
			return
		}

		var capacity int32 = 0
		for _, team := range ptc.Teams {
			capacity += team
		}
		if capacity < 2 {
			ctx.Send(ctx.Self(), &response.CreateRoom{
				Error: enum.ResultCode.NotEnoughUsers, // 수용 유저는 2명 이상이어야 함
			})
			return
		}

		createRoomFuture := ctx.RequestFuture(ctx.Parent(), &msg.RequestCreateRoom{
			ID:    state.ID,
			Title: ptc.Title,
			Teams: ptc.Teams,
		}, time.Hour)
		ctx.AwaitFuture(createRoomFuture, func(res interface{}, err error) {
			createRoom := res.(*msg.ResponseCreateRoom)
			state.Room = createRoom.Room
			ctx.Send(ctx.Self(), &response.CreateRoom{
				Id: createRoom.ID,
			})
		})

	case *request.Chat:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.Chat{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
			return
		}

		message := ptc.Message
		usersFuture := ctx.RequestFuture(state.Room, &msg.RequestGetUsers{}, time.Hour)
		ctx.AwaitFuture(usersFuture, func(res interface{}, err error) {
			users := res.(*msg.ResponseGetUsers)
			for _, user := range users.Users {
				ctx.Send(user.PID, &msg.Chat{
					User:    state.ID,
					Message: message,
				})
			}
		})

	case *request.Whisper:
		to := ptc.User
		message := ptc.Message
		userFuture := ctx.RequestFuture(ctx.Parent(), &msg.RequestGetUser{
			ID: to,
		}, time.Hour)
		ctx.AwaitFuture(userFuture, func(res interface{}, err error) {
			if err != nil {
				ctx.Send(ctx.Self(), &response.Whisper{
					Error: enum.ResultCode.InvalidUser,
				})
				return
			}

			user := res.(*msg.ResponseGetUser)
			whisper := &msg.Whisper{
				From:    state.ID,
				To:      to,
				Message: message,
			}
			ctx.Send(ctx.Self(), whisper)
			ctx.Send(user.User, whisper)
		})

	case *request.KickRoom:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.KickRoom{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
			return
		}

		userFuture := ctx.RequestFuture(ctx.Parent(), &msg.RequestGetUser{
			ID: ptc.User,
		}, time.Hour)
		ctx.AwaitFuture(userFuture, func(res interface{}, err error) {
			if err != nil {
				return
			}

			user := res.(*msg.ResponseGetUser)
			if state.Room == nil {
				ctx.Send(ctx.Self(), &response.KickRoom{
					Error: enum.ResultCode.NotEnteredAnyGameRoom,
				})
				return
			}

			ctx.Send(state.Room, &msg.RequestKick{
				From: ctx.Self(),
				To:   user.User,
			})
		})

	case *request.LeaveRoom:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.LeaveRoom{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
			return
		}

		ctx.Send(state.Room, &msg.Leave{
			User: ctx.Self(),
			UID:  state.ID,
		})

	case *request.GameStart:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.GameStart{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
		}

		ctx.Send(state.Room, &msg.GameStart{
			Sender: ctx.Self(),
		})

	case *request.ActionQueue:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.ActionQueue{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
			return
		}

		ctx.Send(state.Room, &msg.Action{
			Sender:  ctx.Self(),
			UID:     state.ID,
			Actions: ptc.Actions,
			Turn:    ptc.Turn,
		})

		log.Printf("turn (from %d) : %s", ptc.Turn, state.ID)

	case *request.InGameChat:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.ActionQueue{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
			return
		}

		ctx.Send(state.Room, &msg.InGameChat{
			Sender:  ctx.Self(),
			UID:     state.ID,
			Message: ptc.Message,
			Frame:   ptc.Frame,
			Turn:    ptc.Frame,
		})

		log.Printf("ingame chat (from %d) : %s", ptc.Turn, state.ID)

	case *request.Ready:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.ActionQueue{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
			return
		}

		ctx.Send(state.Room, &msg.Ready{})
	}
}

func (state *Actor) Receive(ctx actor.Context) {
	switch m := ctx.Message().(type) {
	case *actor.Started:
		state.session = ctx.Spawn(actor.PropsFromProducer(func() actor.Actor {
			return session.New(state.conn)
		}))

		ctx.Send(ctx.Self(), &response.Login{
			Id: state.ID,
		})

	case *actor.Terminated:
		ctx.Send(ctx.Parent(), &msg.Disconnected{
			ID: state.ID,
		})

		if state.Room != nil {
			ctx.Send(state.Room, &msg.Leave{
				User: ctx.Self(),
				UID:  state.ID,
			})
		}

	case *receiver.Disconnected:
		ctx.Poison(ctx.Self())

	case *msg.RequestGetUserState:
		ctx.Respond(&msg.ResponseGetUserState{
			PID: ctx.Self(),
			User: msg.User{
				ID:  state.ID,
				PID: ctx.Self(),
			},
		})

	case *msg.Whisper:
		ctx.Send(ctx.Self(), &response.Whisper{
			From:    m.From,
			To:      m.To,
			Message: m.Message,
		})

	case *msg.Kicked:
		ctx.Send(ctx.Self(), &response.KickedRoom{})

	case *msg.LeftSelf:
		ctx.Send(ctx.Self(), &response.LeaveRoom{
			User: state.ID,
		})

		state.Room = nil

	case *msg.Left:
		msg := &response.LeaveRoom{
			User: m.UID,
		}

		if m.NewMaster != nil {
			msg.NewMaster = m.NewMaster.ID
		}
		ctx.Send(ctx.Self(), msg)

	case *receiver.Received:
		state.onReceiveFlatBuffer(ctx, m.Protocol)

	case protocol.Protocol:
		ctx.Send(state.session, &sender.Send{Protocol: m})
	}
}
