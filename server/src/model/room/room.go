package room

import (
	"crypto/rand"
	"enum"
	"exception"
	"log"
	"math"
	"math/big"
	"msg"
	"protocol/response"
	"sort"
	"time"

	"github.com/AsynkronIT/protoactor-go/actor"
)

type RoomConfig struct {
	Team  []int32
	Title string
}

type Actor struct {
	id      string
	users   map[int]*actor.PIDSet
	playing bool
	config  RoomConfig
	master  *actor.PID
}

func New(id string, master *actor.PID, config RoomConfig) *Actor {
	result := &Actor{
		id:      id,
		users:   make(map[int]*actor.PIDSet),
		playing: false,
		config:  config,
		master:  master,
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

func (state *Actor) selects() *actor.PIDSet {
	result := actor.NewPIDSet()
	for _, userSet := range state.users {
		userSet.ForEach(func(i int, pid *actor.PID) {
			result.Add(pid)
		})
	}

	return result
}

func (state *Actor) sendStateResponse(ctx actor.Context, fn func(users []msg.UserState, master msg.User) interface{}) {

	var master *msg.User = nil
	users := []msg.UserState{}
	teams := map[*actor.PID]int{}
	gathered := 0
	requests := state.selects().Len()
	for team, pidSet := range state.users {
		pidSet.ForEach(func(i int, pid *actor.PID) {
			teams[pid] = team

			// 각각의 유저들의 정보를 요청
			future := ctx.RequestFuture(pid, &msg.RequestGetUserState{}, time.Hour)
			ctx.AwaitFuture(future, func(res interface{}, err error) {
				if err != nil {
					log.Fatalf("Failed to handle, message : %#v.", ctx.Message())
					return
				}

				// 해당 유저 정보를 응답 메시지에 저장
				x := res.(*msg.ResponseGetUserState)
				users = append(users, msg.UserState{
					User: x.User,
					Team: teams[x.PID],
				})
				gathered++

				if x.PID == state.master {
					master = &x.User
				}

				// 모든 유저 정보 수집이 끝나면 응답 메시지 전달
				if gathered == requests {
					ctx.Respond(fn(users, *master))
				}
			})
		})
	}
}

func (state *Actor) placeable() (int, exception.Error) {
	sorted := []int{}
	for team := range state.config.Team {
		sorted = append(sorted, team)
	}

	sort.Slice(sorted, func(i, j int) bool {
		return state.users[i].Len() < state.users[j].Len()
	})

	for _, team := range sorted {
		current := state.users[team].Len()
		capacity := state.config.Team[team]

		if current < int(capacity) {
			return team, nil
		}
	}

	return 0, exception.New(enum.ResultCode.FullUsers)
}

func (state *Actor) placed(user *actor.PID) (int, exception.Error) {
	for i, team := range state.users {
		if team.Contains(user) {
			return i, nil
		}
	}

	return 0, exception.New(enum.ResultCode.InvalidUser)
}

func (state *Actor) contains(pid *actor.PID) bool {
	users := state.selects()
	return users.Contains(pid)
}

func (state *Actor) playable() exception.Error {
	users := state.selects()

	if state.playing {
		return exception.New(enum.ResultCode.AlreadyPlaying)
	}

	// 두명 이상 있어야함
	if users.Len() < 2 {
		return exception.New(enum.ResultCode.NotEnoughUsers)
	}

	// 1명 이상 참여한 팀이 2개 이상 있어야함
	availables := map[int]int{}
	for team, pidSet := range state.users {
		availables[team] = pidSet.Len()
	}

	if len(availables) < 2 {
		return exception.New(enum.ResultCode.NotEnoughTeams)
	}

	return nil
}

func (state *Actor) Receive(ctx actor.Context) {
	switch x := ctx.Message().(type) {
	case *msg.RequestGetRoomState:
		state.sendStateResponse(ctx, func(users []msg.UserState, master msg.User) interface{} {
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
		users := state.selects()
		if users.Contains(x.Sender) {
			ctx.Respond(&msg.ResponseEnterRoom{
				Error: exception.New(enum.ResultCode.AlreadyEnteredGameRoom),
			})
			return
		}

		i, err := state.placeable()
		if err != nil {
			ctx.Respond(&msg.ResponseEnterRoom{
				Error: err,
			})
			return
		}

		state.users[i].Add(x.Sender)
		state.sendStateResponse(ctx, func(users []msg.UserState, master msg.User) interface{} {
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
		if !state.contains(x.User) {
			return
		}

		i, err := state.placed(x.User)
		if err != nil {
			return
		}

		state.users[i].Remove(x.User)
		ctx.Send(x.User, &msg.LeftSelf{})

		var master *actor.PID
		users := state.selects()
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
			users.ForEach(func(i int, pid *actor.PID) {
				ctx.Send(pid, result)
			})
		} else {

			future := ctx.RequestFuture(master, &msg.RequestGetUserState{}, time.Hour)
			ctx.AwaitFuture(future, func(res interface{}, err error) {
				if err != nil {
					return
				}

				x := res.(*msg.ResponseGetUserState)
				result.NewMaster = &x.User

				users.ForEach(func(i int, pid *actor.PID) {
					ctx.Send(pid, result)
				})
			})
		}

	case *msg.Chat:
		users := state.selects()
		users.ForEach(func(i int, pid *actor.PID) {
			ctx.Send(pid, &msg.ReceiveChat{
				User:    x.User,
				Message: x.Message,
			})
		})

	case *msg.Kick:
		from := x.From
		to := x.To

		future := ctx.RequestFuture(x.To, &msg.RequestGetUserState{}, time.Hour)
		ctx.AwaitFuture(future, func(res interface{}, err error) {
			if err != nil {
				return
			}

			x := res.(*msg.ResponseGetUserState)
			users := state.selects()
			if state.master != from {
				return
			}

			if !users.Contains(to) {
				return
			}

			users.Remove(to)
			ctx.Send(to, &msg.Kicked{})

			users.ForEach(func(i int, pid *actor.PID) {
				ctx.Send(pid, &msg.Left{
					User: to,
					UID:  x.User.ID,
				})
			})
		})

	case *msg.GameStart:
		if x.Sender == nil {
			return
		}
		users := state.selects()

		result := &response.GameStart{
			Error: 0,
		}

		if !users.Contains(x.Sender) {
			result.Error = enum.ResultCode.InvalidUser
			ctx.Send(x.Sender, result)
			return
		}

		if state.master != x.Sender {
			result.Error = enum.ResultCode.NoPrivilege
			ctx.Send(x.Sender, result)
			return
		}

		// if err := state.playable(); err != nil {
		// 	result.Error = err.Code()
		// 	ctx.Send(x.Sender, result)
		// 	return
		// }

		// 랜덤시드 설정
		seed, _ := rand.Int(rand.Reader, big.NewInt(math.MaxInt64))
		result.Seed = seed.Int64()

		state.playing = true
		users.ForEach(func(i int, pid *actor.PID) {
			ctx.Send(pid, result)
		})

	case *msg.Action:
		sender := x.Sender
		if !state.contains(sender) {
			ctx.Send(sender, &response.ActionQueue{
				Error: enum.ResultCode.InvalidUser,
			})
			return
		}

		if !state.playing {
			ctx.Send(sender, &response.ActionQueue{
				Error: enum.ResultCode.NotPlayingState,
			})
			return
		}

		result := &response.ActionQueue{
			User:    x.UID,
			Actions: []response.Action{},
		}
		for _, action := range x.Actions {
			result.Actions = append(result.Actions, response.Action{
				Id:     action.Id,
				Frame:  action.Frame,
				Param1: action.Param1,
				Param2: action.Param2,
			})
		}

		state.selects().ForEach(func(i int, pid *actor.PID) {
			ctx.Send(pid, result)
		})
	}
}
