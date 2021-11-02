package room

import (
	"errors"
	"fmt"
	"log"
	"msg"
	"time"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type RoomConfig struct {
	Team  []int32
	Title string
}

type Actor struct {
	id     string
	users  map[int]*actor.PIDSet
	config RoomConfig
	master *actor.PID
}

func New(id string, master *actor.PID, config RoomConfig) *Actor {
	result := &Actor{
		id:     id,
		users:  make(map[int]*actor.PIDSet),
		config: config,
		master: master,
	}

	for i := 0; i < len(config.Team); i++ {
		result.users[i] = actor.NewPIDSet()
	}

	team, err := result.placeable()
	if err != nil {
		return nil
	}
	result.users[team].Add(master)
	return result
}

func (state *Actor) selectUsers() *actor.PIDSet {
	result := actor.NewPIDSet()
	for _, userSet := range state.users {
		for _, user := range userSet.Values() {
			result.Add(user)
		}
	}

	return result
}

func (state *Actor) sendStateResponse(ctx actor.Context, fn func(users map[int][]msg.UserState, master msg.UserState) interface{}) {

	users := map[int][]msg.UserState{}
	var master *msg.UserState = nil
	gathered := 0
	requests := state.selectUsers().Len()
	for teamId, actorSet := range state.users {
		users[teamId] = []msg.UserState{}

		for _, actor := range actorSet.Values() {

			// 각각의 유저들의 정보를 요청
			future := ctx.RequestFuture(actor, &msg.RequestGetUserState{}, time.Hour)
			ctx.AwaitFuture(future, func(res interface{}, err error) {
				if err != nil {
					log.Fatalf("Failed to handle, message : %#v.", ctx.Message())
					return
				}

				// 해당 유저 정보를 응답 메시지에 저장
				x := res.(*msg.ResponseGetUserState)
				users[teamId] = append(users[teamId], x.State)
				gathered++

				if x.PID == state.master {
					master = &x.State
				}

				// 모든 유저 정보 수집이 끝나면 응답 메시지 전달
				if gathered == requests {
					ctx.Respond(fn(users, *master))
				}
			})
		}
	}
}

func (state *Actor) placeable() (int, error) {
	for i, capacity := range state.config.Team {
		if state.users[i].Len() < int(capacity) {
			return i, nil
		}
	}

	return 0, errors.New("no place")
}

func (state *Actor) placed(user *actor.PID) (int, error) {
	for i, team := range state.users {
		if team.Contains(user) {
			return i, nil
		}
	}

	msg := fmt.Sprintf("not found user : %v", user.Id)
	return 0, errors.New(msg)
}

func (state *Actor) Receive(ctx actor.Context) {
	switch x := ctx.Message().(type) {
	case *msg.RequestGetRoomState:
		state.sendStateResponse(ctx, func(users map[int][]msg.UserState, master msg.UserState) interface{} {
			result := &msg.ResponseGetRoomState{
				PID: ctx.Self(),
				State: msg.RoomConfig{
					ID:    state.id,
					Title: state.config.Title,
					Teams: state.config.Team,
				},
				Users:  users,
				Master: master,
				Teams:  state.users,
			}

			return result
		})

	case *msg.RequestEnterRoom:
		users := state.selectUsers()
		if users.Contains(x.Sender) {
			return
		}

		i, err := state.placeable()
		if err != nil {
			ctx.Respond(&msg.ResponseEnterRoom{
				Error: 1, // 참여 불가능
			})
			return
		}

		state.users[i].Add(x.Sender)
		state.sendStateResponse(ctx, func(users map[int][]msg.UserState, master msg.UserState) interface{} {
			return &msg.ResponseEnterRoom{
				PID: ctx.Self(),
				RoomState: msg.RoomConfig{
					ID:    state.id,
					Title: state.config.Title,
					Teams: state.config.Team,
				},
				Users:  users,
				Master: master,
			}
		})

	case *msg.Leave:
		users := state.selectUsers()
		if !users.Contains(x.User) {
			return
		}

		i, err := state.placed(x.User)
		if err != nil {
			return
		}

		state.users[i].Remove(x.User)
		ctx.Send(x.User, &msg.LeftSelf{})

		var master *actor.PID
		if users.Empty() { // 남은 인원 0명인 경우
			ctx.Send(ctx.Parent(), &msg.DestroyRoom{
				ID: state.id,
			})
			ctx.Poison(ctx.Self())
			return
		} else if state.master == x.User { // 퇴장한 유저가 방장인 경우
			master = users.Values()[0]
		} else {
			master = nil
		}

		result := &msg.Left{
			User: x.User,
			UID:  x.UID,
		}
		if master == nil {

			for _, actor := range users.Values() {
				ctx.Send(actor, result)
			}
		} else {

			future := ctx.RequestFuture(master, &msg.RequestGetUserState{}, time.Second)
			ctx.AwaitFuture(future, func(res interface{}, err error) {
				if err != nil {
					return
				}

				x := res.(*msg.ResponseGetUserState)
				result.NewMaster = &x.State

				for _, actor := range users.Values() {
					ctx.Send(actor, result)
				}
			})
		}

	case *msg.Chat:
		users := state.selectUsers()
		for _, actor := range users.Values() {
			ctx.Send(actor, &msg.ReceiveChat{
				User:    x.User,
				Message: x.Message,
			})
		}

	case *msg.Kick:
		from := x.From
		to := x.To

		future := ctx.RequestFuture(x.To, &msg.RequestGetUserState{}, time.Second)
		ctx.AwaitFuture(future, func(res interface{}, err error) {
			if err != nil {
				return
			}

			x := res.(*msg.ResponseGetUserState)
			users := state.selectUsers()
			if state.master != from {
				return
			}

			if !users.Contains(to) {
				return
			}

			users.Remove(to)
			ctx.Send(to, &msg.Kicked{})

			for _, user := range users.Values() {
				ctx.Send(user, &msg.Left{
					User: to,
					UID:  x.State.ID,
				})
			}
		})
	}
}
