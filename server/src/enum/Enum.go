// DO NOT MODIFY

package enum

type _ClientExceptionCode struct {
	InvalidCellAccess   uint32
	NotWalkableNextCell uint32
	ZeroCellPath        uint32
	NotFoundUIAttribute uint32
	NotContainUIScript  uint32
}

type _ResultCode struct {
	None                   uint32
	InvalidUser            uint32
	NoPrivilege            uint32
	AlreadyPlaying         uint32
	NotEnoughUsers         uint32
	NotEnoughTeams         uint32
	NotFoundGameRoom       uint32
	AlreadyEnteredGameRoom uint32
	NotEnteredAnyGameRoom  uint32
	FullUsers              uint32
	NotPlayingState        uint32
}

type _ActionKind struct {
	None           uint32
	Pause          uint32
	Resume         uint32
	SetDestination uint32
	AttackTo       uint32
}

type _ProjectileState struct {
	Disable uint32
	Shoot   uint32
	Move    uint32
	Hit     uint32
}

type _ProjectileType struct {
	Absolute uint32
	Relative uint32
}

type _UnitSize struct {
	Small  uint32
	Medium uint32
	Large  uint32
}

type _UnitState struct {
	Idle   uint32
	Move   uint32
	Attack uint32
	Dead   uint32
}

type _UnitType struct {
	Normal     uint32
	Explosive  uint32
	Concussive uint32
}

type _StatType struct {
	Hp          uint32
	Damage      uint32
	Armor       uint32
	AttackRange uint32
	Speed       uint32
	AttackSpeed uint32
}

type _CommandType struct {
	Move               uint32
	AttackSingleTarget uint32
	AttackMultiTarget  uint32
}

type _AttackType struct {
	Immediately uint32
	Projectile  uint32
}

type _Ability struct {
	NONE       uint32
	UPGRADE_1  uint32
	UPGRADE_2  uint32
	UPGRADE_3  uint32
	UPGRADE_4  uint32
	UPGRADE_5  uint32
	UPGRADE_6  uint32
	UPGRADE_7  uint32
	UPGRADE_8  uint32
	UPGRADE_9  uint32
	UPGRADE_10 uint32
	UPGRADE_11 uint32
	UPGRADE_12 uint32
	UPGRADE_13 uint32
	UPGRADE_14 uint32
	UPGRADE_15 uint32
	UPGRADE_16 uint32
	UPGRADE_17 uint32
	UPGRADE_18 uint32
	UPGRADE_19 uint32
	UPGRADE_20 uint32
}

type _Advanced struct {
	UPGRADE_WEAPON uint32
	UPGRADE_ARMOR  uint32
	UPGRADE_SPEED  uint32
}

var ClientExceptionCode _ClientExceptionCode
var ResultCode _ResultCode
var ActionKind _ActionKind
var ProjectileState _ProjectileState
var ProjectileType _ProjectileType
var UnitSize _UnitSize
var UnitState _UnitState
var UnitType _UnitType
var StatType _StatType
var CommandType _CommandType
var AttackType _AttackType
var Ability _Ability
var Advanced _Advanced

func init() {
	ClientExceptionCode = _ClientExceptionCode{
		InvalidCellAccess:   1,
		NotWalkableNextCell: 2,
		ZeroCellPath:        3,
		NotFoundUIAttribute: 4,
		NotContainUIScript:  5,
	}

	ResultCode = _ResultCode{
		None:                   1,
		InvalidUser:            2,
		NoPrivilege:            3,
		AlreadyPlaying:         4,
		NotEnoughUsers:         5,
		NotEnoughTeams:         6,
		NotFoundGameRoom:       7,
		AlreadyEnteredGameRoom: 8,
		NotEnteredAnyGameRoom:  9,
		FullUsers:              10,
		NotPlayingState:        11,
	}

	ActionKind = _ActionKind{
		None:           1,
		Pause:          2,
		Resume:         3,
		SetDestination: 4,
		AttackTo:       5,
	}

	ProjectileState = _ProjectileState{
		Disable: 1,
		Shoot:   2,
		Move:    3,
		Hit:     4,
	}

	ProjectileType = _ProjectileType{
		Absolute: 1,
		Relative: 2,
	}

	UnitSize = _UnitSize{
		Small:  1,
		Medium: 2,
		Large:  3,
	}

	UnitState = _UnitState{
		Idle:   1,
		Move:   2,
		Attack: 3,
		Dead:   4,
	}

	UnitType = _UnitType{
		Normal:     1,
		Explosive:  2,
		Concussive: 3,
	}

	StatType = _StatType{
		Hp:          1,
		Damage:      2,
		Armor:       3,
		AttackRange: 4,
		Speed:       5,
		AttackSpeed: 6,
	}

	CommandType = _CommandType{
		Move:               1,
		AttackSingleTarget: 2,
		AttackMultiTarget:  3,
	}

	AttackType = _AttackType{
		Immediately: 1,
		Projectile:  2,
	}

	Ability = _Ability{
		NONE:       0x00000000,
		UPGRADE_1:  0x00000001,
		UPGRADE_2:  0x00000002,
		UPGRADE_3:  0x00000004,
		UPGRADE_4:  0x00000008,
		UPGRADE_5:  0x00000010,
		UPGRADE_6:  0x00000020,
		UPGRADE_7:  0x00000040,
		UPGRADE_8:  0x00000080,
		UPGRADE_9:  0x00000100,
		UPGRADE_10: 0x00000200,
		UPGRADE_11: 0x00000400,
		UPGRADE_12: 0x00000800,
		UPGRADE_13: 0x00001000,
		UPGRADE_14: 0x00002000,
		UPGRADE_15: 0x00004000,
		UPGRADE_16: 0x00008000,
		UPGRADE_17: 0x00010000,
		UPGRADE_18: 0x00020000,
		UPGRADE_19: 0x00040000,
		UPGRADE_20: 0x00080000,
	}

	Advanced = _Advanced{
		UPGRADE_WEAPON: 1,
		UPGRADE_ARMOR:  2,
		UPGRADE_SPEED:  3,
	}
}
