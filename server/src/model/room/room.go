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
	id           string
	users        map[int]*actor.PIDSet
	playing      bool
	config       RoomConfig
	master       *actor.PID
	seed         int64
	nextSequence int32
	mapSequence  map[int32]*actor.PID
}

func New(id string, master *actor.PID, config RoomConfig) *Actor {
	result := &Actor{
		id:           id,
		users:        make(map[int]*actor.PIDSet),
		playing:      false,
		config:       config,
		master:       master,
		seed:         0,
		nextSequence: 0,
		mapSequence:  make(map[int32]*actor.PID),
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

func (state *Actor) getUsers(ctx actor.Context) {
	var master *msg.User = nil
	users := []msg.UserState{}
	teams := map[*actor.PID]int32{}
	gathered := 0
	requests := state.selects().Len()
	for team, pidSet := range state.users {
		pidSet.ForEach(func(i int, pid *actor.PID) {
			teams[pid] = int32(team)

			// 각각의 유저들의 정보를 요청
			userStateFuture := ctx.RequestFuture(pid, &msg.RequestGetUserState{}, time.Hour)
			ctx.AwaitFuture(userStateFuture, func(res interface{}, e error) {
				if e != nil {
					log.Fatalf("Failed to handle, message : %#v.", ctx.Message())
					return
				}

				// 해당 유저 정보를 응답 메시지에 저장
				x := res.(*msg.ResponseGetUserState)
				seq := state.pid2Sequence(x.User.PID)
				users = append(users, msg.UserState{
					User:     x.User,
					Team:     teams[x.PID],
					Sequence: seq,
				})
				gathered++

				if x.PID == state.master {
					master = &x.User
				}

				// 모든 유저 정보 수집이 끝나면 응답 메시지 전달
				if gathered == requests {
					ctx.Respond(&msg.ResponseGetUsers{
						Master: *master,
						Users:  users,
					})
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

func (state *Actor) assertPlay(sender *actor.PID) uint32 {
	if !state.contains(sender) {
		return enum.ResultCode.InvalidUser
	}

	if !state.playing {
		return enum.ResultCode.NotPlayingState
	}

	return enum.ResultCode.None
}

func (state *Actor) pid2Sequence(actor *actor.PID) int32 {
	for seq, pid := range state.mapSequence {
		if pid == actor {
			return seq
		}
	}

	return -1
}

func (state *Actor) Receive(ctx actor.Context) {
	switch m := ctx.Message().(type) {

	case *msg.RequestGetUsers:
		state.getUsers(ctx)

	case *msg.RequestGetRoomState:
		usersFuture := ctx.RequestFuture(ctx.Self(), &msg.RequestGetUsers{}, time.Hour)

		ctx.AwaitFuture(usersFuture, func(res interface{}, err error) {
			if err != nil {
				log.Fatalf("Failed to handle, message : %#v.", ctx.Message())
				return
			}

			users := res.(*msg.ResponseGetUsers)
			result := &msg.ResponseGetRoomState{
				PID: ctx.Self(),
				State: msg.RoomConfig{
					ID:    state.id,
					Title: state.config.Title,
					Teams: state.config.Team,
				},
				Users:  users.Users,
				Master: users.Master,
				Teams:  state.users,
			}

			ctx.Respond(result)
		})

	case *msg.RequestEnterRoom:
		users := state.selects()
		if users.Contains(m.Sender) {
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

		state.users[i].Add(m.Sender)
		usersFuture := ctx.RequestFuture(ctx.Self(), &msg.RequestGetUsers{}, time.Hour)
		ctx.AwaitFuture(usersFuture, func(res interface{}, err error) {
			if err != nil {
				log.Fatalf("Failed to handle, message : %#v.", ctx.Message())
				return
			}

			users := res.(*msg.ResponseGetUsers)
			ctx.Respond(&msg.ResponseEnterRoom{
				PID: ctx.Self(),
				RoomState: msg.RoomConfig{
					ID:    state.id,
					Title: state.config.Title,
					Teams: state.config.Team,
				},
				Users:  users.Users,
				Master: users.Master,
			})
		})

	case *msg.Leave:
		if !state.contains(m.User) {
			return
		}

		i, err := state.placed(m.User)
		if err != nil {
			return
		}

		state.users[i].Remove(m.User)
		ctx.Send(m.User, &msg.LeftSelf{})

		var master *actor.PID
		users := state.selects()
		if users.Empty() { // 남은 인원 0명인 경우
			ctx.Send(ctx.Parent(), &msg.DestroyRoom{
				ID: state.id,
			})
			ctx.Poison(ctx.Self())
			return
		} else if state.master == m.User { // 퇴장한 유저가 방장인 경우
			master = users.Values()[0]
		} else {
			master = nil
		}

		result := &msg.Left{
			User: m.User,
			UID:  m.UID,
		}
		if master == nil {
			users.ForEach(func(i int, pid *actor.PID) {
				ctx.Send(pid, result)
			})
		} else {

			userStateFuture := ctx.RequestFuture(master, &msg.RequestGetUserState{}, time.Hour)
			ctx.AwaitFuture(userStateFuture, func(res interface{}, err error) {
				if err != nil {
					return
				}

				userState := res.(*msg.ResponseGetUserState)
				result.NewMaster = &userState.User

				users.ForEach(func(i int, pid *actor.PID) {
					ctx.Send(pid, result)
				})
			})
		}

	case *msg.RequestKick:
		from := m.From
		to := m.To

		userStateFuture := ctx.RequestFuture(m.To, &msg.RequestGetUserState{}, time.Hour)
		ctx.AwaitFuture(userStateFuture, func(res interface{}, err error) {
			if err != nil {
				return
			}

			userState := res.(*msg.ResponseGetUserState)
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
					UID:  userState.User.ID,
				})
			})
		})

	case *msg.GameStart:
		if m.Sender == nil {
			return
		}
		users := state.selects()

		result := &response.GameStart{
			Error: 0,
		}

		if !users.Contains(m.Sender) {
			result.Error = enum.ResultCode.InvalidUser
			ctx.Send(m.Sender, result)
			return
		}

		if state.master != m.Sender {
			result.Error = enum.ResultCode.NoPrivilege
			ctx.Send(m.Sender, result)
			return
		}

		// if err := state.playable(); err != nil {
		// 	result.Error = err.Code()
		// 	ctx.Send(x.Sender, result)
		// 	return
		// }

		state.playing = true
		users.ForEach(func(i int, pid *actor.PID) {
			ctx.Send(pid, result)
		})

		// 랜덤시드 설정
		seed, _ := rand.Int(rand.Reader, big.NewInt(math.MaxInt64))
		state.seed = seed.Int64()

	case *msg.Ready:
		sender := ctx.Sender()
		state.mapSequence[state.nextSequence] = sender
		state.nextSequence++

		sequences := make(map[int32]string)
		usersFuture := ctx.RequestFuture(ctx.Self(), &msg.RequestGetUsers{}, time.Hour)
		ctx.AwaitFuture(usersFuture, func(res interface{}, err error) {
			usersResponse := res.(*msg.ResponseGetUsers)

			users := []response.User{}
			for _, user := range usersResponse.Users {
				users = append(users, response.User{
					Id:   user.ID,
					Team: int32(user.Team),
				})

				seq := state.pid2Sequence(user.PID)
				sequences[seq] = user.ID
			}

			state.selects().ForEach(func(i int, pid *actor.PID) {
				ctx.Send(pid, &response.Ready{
					Seed:  state.seed,
					Users: users,
				})
			})
		})

	case *msg.Action:
		sender := ctx.Sender()
		if err := state.assertPlay(sender); err != enum.ResultCode.None {
			ctx.Send(sender, &response.ActionQueue{
				Error: err,
			})
			return
		}

		result := &response.ActionQueue{
			User:    state.pid2Sequence(sender),
			Actions: []response.Action{},
			Turn:    m.Turn,
		}
		for _, action := range m.Actions {
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

	case *msg.InGameChat:
		sender := ctx.Sender()
		if err := state.assertPlay(sender); err != enum.ResultCode.None {
			ctx.Send(sender, &response.ActionQueue{
				Error: err,
			})
			return
		}

		state.selects().ForEach(func(i int, pid *actor.PID) {
			ctx.Send(pid, &response.InGameChat{
				Turn:    m.Turn,
				Frame:   m.Frame,
				User:    state.pid2Sequence(pid),
				Message: m.Message,
			})
		})

	case *msg.Resume:
		if err := state.assertPlay(ctx.Sender()); err != enum.ResultCode.None {
			ctx.Send(ctx.Sender(), &response.Resume{
				Error: err,
			})
			return
		}

		state.selects().ForEach(func(i int, pid *actor.PID) {
			ctx.Send(pid, &response.Resume{
				User: m.User,
			})
		})
	}
}
