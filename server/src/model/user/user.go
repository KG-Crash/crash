package user

import (
	"enum"
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

	switch x := p.(type) {
	case *request.EnterRoom:
		if state.Room != nil {
			ctx.Send(ctx.Self(), &response.EnterRoom{
				Error: enum.ResultCode.AlreadyEnteredGameRoom,
			})
			return
		}

		future := ctx.RequestFuture(ctx.Parent(), &msg.RequestGetRoom{
			ID: x.Id,
		}, time.Hour)
		ctx.AwaitFuture(future, func(res interface{}, err error) {
			x := res.(*msg.ResponseGetRoom)
			future := ctx.RequestFuture(x.Room, &msg.RequestEnterRoom{
				Sender: ctx.Self(),
			}, time.Hour)
			ctx.AwaitFuture(future, func(res interface{}, err error) {
				x := res.(*msg.ResponseEnterRoom)

				result := &response.EnterRoom{
					User:   state.ID,
					Users:  toUserResponses(x.Users),
					Master: x.Master.ID,
				}

				for _, user := range x.Users {
					ctx.Send(user.PID, result)
				}
			})
		})

	case *request.RoomList:
		future := ctx.RequestFuture(ctx.Parent(), &msg.RequestGetRoomList{}, time.Hour)
		ctx.AwaitFuture(future, func(res interface{}, err error) {

			x := res.(*msg.ResponseGetRoomList)
			result := &response.RoomList{
				Rooms: []response.Room{},
			}

			count := x.Rooms.Len()
			if count > 0 {
				x.Rooms.ForEach(func(i int, pid *actor.PID) {
					future := ctx.RequestFuture(pid, &msg.RequestGetRoomState{}, time.Hour)
					ctx.AwaitFuture(future, func(res interface{}, err error) {

						x := res.(*msg.ResponseGetRoomState)
						result.Rooms = append(result.Rooms, response.Room{
							Id:    x.State.ID,
							Title: x.State.Title,
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

		if len(x.Teams) < 2 {
			ctx.Send(ctx.Self(), &response.CreateRoom{
				Error: enum.ResultCode.NotEnoughTeams, // 팀은 2개 이상 설정되어야 함
			})
			return
		}

		var capacity int32 = 0
		for _, team := range x.Teams {
			capacity += team
		}
		if capacity < 2 {
			ctx.Send(ctx.Self(), &response.CreateRoom{
				Error: enum.ResultCode.NotEnoughUsers, // 수용 유저는 2명 이상이어야 함
			})
			return
		}

		future := ctx.RequestFuture(ctx.Parent(), &msg.RequestCreateRoom{
			ID:    state.ID,
			Title: x.Title,
			Teams: x.Teams,
		}, time.Hour)
		ctx.AwaitFuture(future, func(res interface{}, err error) {
			x := res.(*msg.ResponseCreateRoom)
			state.Room = x.Room
			ctx.Send(ctx.Self(), &response.CreateRoom{
				Id: x.ID,
			})
		})

	case *request.Chat:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.Chat{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
			return
		}

		ctx.Send(state.Room, &msg.Chat{
			User:    state.ID,
			Message: x.Message,
		})

	case *request.Whisper:
		to := x.User
		message := x.Message
		future := ctx.RequestFuture(ctx.Parent(), &msg.RequestGetUser{
			ID: to,
		}, time.Hour)
		ctx.AwaitFuture(future, func(res interface{}, err error) {
			if err != nil {
				ctx.Send(ctx.Self(), &response.Whisper{
					Error: enum.ResultCode.InvalidUser,
				})
				return
			}

			x := res.(*msg.ResponseGetUser)
			msg := &msg.Whisper{
				From:    state.ID,
				To:      to,
				Message: message,
			}
			ctx.Send(ctx.Self(), msg)
			ctx.Send(x.User, msg)
		})

	case *request.KickRoom:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.LeaveRoom{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
			return
		}

		future := ctx.RequestFuture(ctx.Parent(), &msg.RequestGetUser{
			ID: x.User,
		}, time.Hour)
		ctx.AwaitFuture(future, func(res interface{}, err error) {
			if err != nil {
				return
			}

			x := res.(*msg.ResponseGetUser)
			if state.Room == nil {
				ctx.Send(ctx.Self(), &response.KickRoom{
					Error: enum.ResultCode.NotEnteredAnyGameRoom,
				})
				return
			}

			ctx.Send(state.Room, &msg.Kick{
				From: ctx.Self(),
				To:   x.User,
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
			Actions: x.Actions,
		})

	case *request.Ready:
		if state.Room == nil {
			ctx.Send(ctx.Self(), &response.ActionQueue{
				Error: enum.ResultCode.NotEnteredAnyGameRoom,
			})
			return
		}

		future := ctx.RequestFuture(state.Room, &msg.RequestReady{}, time.Hour)
		ctx.AwaitFuture(future, func(res interface{}, err error) {
			x := res.(*msg.ResponseReady)

			ctx.Send(ctx.Self(), &response.Ready{
				Seed:  x.Seed,
				Users: toUserResponses(x.Users),
			})
		})
	}
}

func (state *Actor) Receive(ctx actor.Context) {
	switch x := ctx.Message().(type) {
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

	case *msg.ReceiveChat:
		ctx.Send(ctx.Self(), &response.Chat{
			User:    x.User,
			Message: x.Message,
		})

	case *msg.Whisper:
		ctx.Send(ctx.Self(), &response.Whisper{
			From:    x.From,
			To:      x.To,
			Message: x.Message,
		})

	case *msg.Kicked:
		ctx.Send(ctx.Self(), &response.KickedRoom{})

	case *msg.LeftSelf:
		ctx.Send(ctx.Self(), &response.LeaveRoom{
			User: state.ID,
		})

	case *msg.Left:
		msg := &response.LeaveRoom{
			User: x.UID,
		}

		if x.NewMaster != nil {
			msg.NewMaster = x.NewMaster.ID
		}
		ctx.Send(ctx.Self(), msg)

	case *receiver.Received:
		state.onReceiveFlatBuffer(ctx, x.Protocol)

	case protocol.Protocol:
		ctx.Send(state.session, &sender.Send{Protocol: x})
	}
}
