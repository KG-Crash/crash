package main

import (
	"context"

	"github.com/KG-Crash/crash/lobby/controller/auth"
	"github.com/KG-Crash/crash/lobby/controller/lobby"

	"github.com/gin-gonic/gin"
)

var ctx = context.Background()
var hmacSecret = []byte("kg-crash")

func main() {
	r := gin.Default()
	auth.Bind(r)
	lobby.Bind(r.Group("/lobby"))
	r.Run(":8001")
}
