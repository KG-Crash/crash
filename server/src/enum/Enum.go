package enum

type _ItemType struct {
	Equipment uint32
	Consume   uint32
	Other     uint32
}

type _EquipmentType struct {
	Weapon    uint32
	Shield    uint32
	Armor     uint32
	Shoes     uint32
	Helmet    uint32
	Accessory uint32
}

type _WeaponType struct {
	Sword uint32
	Bow   uint32
	Staff uint32
}

var ItemType _ItemType
var EquipmentType _EquipmentType
var WeaponType _WeaponType

func init() {
	ItemType = _ItemType{
		Equipment: 1, // 장비
		Consume:   2, // 소비
		Other:     3, // 기타
	}

	EquipmentType = _EquipmentType{
		Weapon:    1, // 무기
		Shield:    2, // 방패
		Armor:     3, // 갑옷
		Shoes:     4, // 신발
		Helmet:    5, // 모자
		Accessory: 6, // 장신구
	}

	WeaponType = _WeaponType{
		Sword: 1, // 검
		Bow:   2, // 활
		Staff: 3, // 지팡이
	}
}
