package main

import (
	"db"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"os"
)

type _Configuration struct {
	Port  uint16
	Mysql map[string]db.DBConfig
}

var Configuration _Configuration

func init() {
	env := os.Getenv("CRASH_ENVIRONMENT")
	if env == "" {
		env = "local"
	}

	path := fmt.Sprintf("appsettings.%s.json", env)
	if data, err := ioutil.ReadFile(path); err == nil {
		json.Unmarshal(data, &Configuration)
		db.Setup(Configuration.Mysql)
	} else {
		log.Fatalf("Cannot load file : %s", path)
	}
}
