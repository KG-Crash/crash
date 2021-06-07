package constant

type _Portal struct {
	Width  int
	Height int
}

type _Character struct {
	InitStat string
}

var Portal _Portal
var Character _Character

func init() {
	Portal = _Portal{
		Width:  1,
		Height: 2,
	}
	Character = _Character{
		InitStat: "캐릭터.스탯",
	}
}
