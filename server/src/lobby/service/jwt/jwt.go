package jwt

import (
	"fmt"

	"github.com/KG-Crash/crash/lobby/service/http"
	"github.com/KG-Crash/crash/protocol/response"
	"github.com/dgrijalva/jwt-go"
	"github.com/gin-gonic/gin"
)

var hmacSecret = []byte("kg-crash")

func Sign(id string) (string, error) {
	token := jwt.NewWithClaims(jwt.SigningMethodHS256, jwt.MapClaims{
		"id": id,
	})

	tokenString, err := token.SignedString(hmacSecret)
	if err != nil {
		return "", err
	}

	return tokenString, nil
}

func Middleware(ctx *gin.Context) {
	if ctx.Request.RequestURI == "/lobby/authentication" {
		ctx.Next()
		return
	}
	if len(ctx.Request.Header["Authorization"]) != 1 {
		http.SetResponse(ctx, response.HttpException{Error: 1})
		return
	}
	var tokenString string
	_, err := fmt.Sscanf(ctx.Request.Header["Authorization"][0], "Bearer %s", &tokenString)
	if err != nil {
		http.SetResponse(ctx, response.HttpException{Error: 1})
		return
	}
	token, err := jwt.Parse(tokenString, func(t *jwt.Token) (interface{}, error) {
		if _, ok := t.Method.(*jwt.SigningMethodHMAC); !ok {
			return nil, fmt.Errorf("Unexpected signing method: %v", t.Header["alg"])
		}
		return hmacSecret, nil
	})
	claims, ok := token.Claims.(jwt.MapClaims)
	if !ok || !token.Valid {
		http.SetResponse(ctx, response.HttpException{Error: 1})
		return
	}
	id := claims["id"].(string)
	ctx.Request.Header["id"] = []string{id}
	ctx.Next()
}
