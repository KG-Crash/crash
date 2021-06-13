// DO NOT MODIFY

package table

import (
	"encoding/json"
	"io/ioutil"
)

func init() {

	if data, err := ioutil.ReadFile("json/SampleAttribute.json"); err == nil {
		json.Unmarshal(data, &TableSampleAttribute)
	}

	if data, err := ioutil.ReadFile("json/Sample.json"); err == nil {
		json.Unmarshal(data, &TableSample)
	}

	if data, err := ioutil.ReadFile("json/Sample2.json"); err == nil {
		json.Unmarshal(data, &TableSample2)
	}

	if data, err := ioutil.ReadFile("json/UnitUpgrade.json"); err == nil {
		json.Unmarshal(data, &TableUnitUpgrade)
	}

}

type SampleAttribute struct {
	GroupKey    string
	GroupField1 string
	GroupField2 string
	GroupField3 int
}

var TableSampleAttribute map[string]SampleAttribute

type Sample struct {
	Parent       string
	MemberField1 string
	MemberField2 string
	MemberField3 string
}

var TableSample map[string][]Sample

type Sample2 struct {
	Id   int
	Name string
}

var TableSample2 map[int]Sample2

type UnitUpgrade struct {
	Parent Ability
}

var TableUnitUpgrade map[Ability][]UnitUpgrade
