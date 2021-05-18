package main

import (
	"fmt"

	Response "protocol/Response"
)

func sample() {
	src := Response.CreateRoom{
		Id: 100,
	}
	bytes := src.Serialize()
	fmt.Println(bytes)

	dst := Response.CreateRoom{}
	dst.Deserialize(bytes)
	fmt.Println(dst)
}

func main() {
	sample()
}
