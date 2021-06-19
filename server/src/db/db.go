package db

import (
	"fmt"
	"log"

	"gorm.io/driver/mysql"
	"gorm.io/gorm"
)

type DBConfig struct {
	Host     string
	Port     uint16
	Uid      string
	Pwd      string
	Database string
}

// 이 DB 모델은 샘플임.. 마이그레이션 테스트용...
type User struct {
	Id   uint64 `gorm:"primaryKey;autoIncrement:true"`
	Name string `gorm:"index;default:default name"`
}

type Room struct {
	UserId uint64 `gorm:"index"`
	User   User   `gorm:"foreignKey:Id;references:UserId"`
}

var Context map[string]*gorm.DB

func migrate(db *gorm.DB) {
	db.AutoMigrate(&User{})
	db.AutoMigrate(&Room{})
}

func Setup(configs map[string]DBConfig) map[string]*gorm.DB {
	Context = make(map[string]*gorm.DB)
	for name, config := range configs {
		dsn := fmt.Sprintf("%s:%s@tcp(%s:%d)/%s?charset=utf8mb4&parseTime=True&loc=Local",
			config.Uid, config.Pwd, config.Host, config.Port, config.Database)
		db, err := gorm.Open(mysql.Open(dsn), &gorm.Config{})
		if err != nil {
			log.Fatalf(err.Error())
			continue
		}

		migrate(db)
		Context[name] = db
	}

	return Context
}
