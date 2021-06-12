// DO NOT MODIFY

package enum

type _UnitSize struct {
	Small  uint32
	Medium uint32
	Large  uint32
}

type _UnitType struct {
	Normal     uint32
	Explosive  uint32
	Concussive uint32
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
}

var UnitSize _UnitSize
var UnitType _UnitType
var Ability _Ability
var Advanced _Advanced

func init() {
	UnitSize = _UnitSize{
		Small:  1,
		Medium: 2,
		Large:  3,
	}

	UnitType = _UnitType{
		Normal:     1,
		Explosive:  2,
		Concussive: 3,
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
	}
}
