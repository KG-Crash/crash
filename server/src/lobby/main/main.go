package main

import (
	"fmt"
	"io/ioutil"
	"net/http"

	"github.com/gin-gonic/gin"
)

func main() {
	r := gin.Default()
	r.POST("/ping", func(c *gin.Context) {
		bytes, _ := ioutil.ReadAll(c.Request.Body)
		fmt.Println(bytes)

		byteFile, err := ioutil.ReadFile("./binary.txt")
		if err != nil {
			fmt.Println(err)
		}

		c.Header("Content-Disposition", "attachment; filename=file-name.txt")
		c.Data(http.StatusOK, "application/octet-stream", byteFile)
	})
	r.Run() // listen and serve on 0.0.0.0:8080 (for windows "localhost:8080")
}
