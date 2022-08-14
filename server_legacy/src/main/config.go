package main

import (
	"context"
	"db"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"os"

	"github.com/go-redis/redis"
)

type redisSetting struct {
	Host string
	Port uint16
}

type config struct {
	Port  uint16
	Mysql map[string]db.DBConfig
	Redis redisSetting
}

var Config config

func init() {
	env := os.Getenv("CRASH_ENVIRONMENT")
	if env == "" {
		env = "local"
	}

	// mysql 연결
	path := fmt.Sprintf("appsettings.%s.json", env)
	if data, err := ioutil.ReadFile(path); err == nil {
		json.Unmarshal(data, &Config)
		db.Setup(Config.Mysql)

		ctx := context.Background()
		client := redis.NewClient(&redis.Options{
			Addr: fmt.Sprintf("%s:%d", Config.Redis.Host, Config.Redis.Port),
		})

		pong, err := client.Ping(ctx).Result()
		fmt.Println(pong, err)
		client.Close()
	} else {
		log.Fatalf("Cannot load file : %s", path)
	}
}
