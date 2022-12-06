package setting

import (
	"encoding/json"
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
	Redis  Redis
}

var ist Setting

func init() {
	data, _ := os.Open("setting.json")
	bytes, _ := ioutil.ReadAll(data)
	json.Unmarshal(bytes, &ist)
}

func Get() Setting {
	return ist
}
