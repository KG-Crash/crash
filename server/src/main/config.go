package main

import (
	"db"
	"encoding/json"
	"io/ioutil"
)

type _Configuration struct {
	Mysql map[string]db.DBConfig
}

var Configuration _Configuration

func init() {
	if data, err := ioutil.ReadFile("appsettings.json"); err == nil {
		json.Unmarshal(data, &Configuration)
	}

	db.Setup(Configuration.Mysql)
}
