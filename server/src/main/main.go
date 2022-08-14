package main

import (
	"KG/session"
	"fmt"
	"log"
	"net"
	"os"
	"time"
)

func main() {

	qwe := time.Now()
	fmt.Println(qwe)

	port := 3333

	listen, err := net.Listen("tcp4", fmt.Sprintf(":%d", port))
	if err != nil {
		os.Exit(1)
	}
	defer listen.Close()

	for {
		conn, err := listen.Accept()
		if err != nil {
			log.Fatalln(err)
			continue
		}

		x := session.New(conn)
		go x.Handler()
	}
}
