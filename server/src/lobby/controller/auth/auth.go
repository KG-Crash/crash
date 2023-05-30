package auth

import (
	"github.com/KG-Crash/crash/lobby/service/http"
	"github.com/KG-Crash/crash/lobby/service/jwt"
	"github.com/KG-Crash/crash/protocol/request"
	"github.com/KG-Crash/crash/protocol/response"
	"github.com/gin-gonic/gin"
)

func authentication(ctx *gin.Context) {
	req := http.GetRequest[*request.Authentication](ctx)
	identity := req.Identity()

	if identity != request.AUTHENTICATION {
		http.SetResponse(ctx, response.Authentication{Error: 1})
		return
	}

	token, err := jwt.Sign(req.Id)
	if err != nil {
		http.SetResponse(ctx, response.Authentication{Error: 1})
		return
	}

	http.SetResponse(ctx, response.Authentication{Token: token})
}

func Bind(r *gin.Engine) {
	r.POST("/authentication", authentication)
}
