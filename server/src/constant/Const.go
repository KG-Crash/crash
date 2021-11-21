// DO NOT MODIFY

package constant

type _Portal struct {
	Width  int
	Height int
}

type _Character struct {
	InitStat string
}

type _Input struct {
}

type _Time struct {
}

var Portal _Portal
var Character _Character
var Input _Input
var Time _Time

func init() {
	Portal = _Portal{
		Width:  1,
		Height: 2,
	}
	Character = _Character{
		InitStat: "캐릭터.스탯",
	}
	Input = _Input{}
	Time = _Time{}
}
