package setting

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"os"
)

type Server struct {
	Host string
	Port uint16
}

type Redis struct {
	Host string
	Port uint16
	Db   uint8
}

type Setting struct {
	Server map[string][]Server
	Redis  []Redis
}

var ist Setting

func init() {
	var fname string
	if env, ok := os.LookupEnv("CRASH_ENVIRONMENT"); ok {
		fname = fmt.Sprintf("setting.%s.json", env)
	} else {
		fname = "setting.json"
	}
	data, _ := os.Open(fname)
	bytes, _ := ioutil.ReadAll(data)
	json.Unmarshal(bytes, &ist)
}

func Get() Setting {
	return ist
}
