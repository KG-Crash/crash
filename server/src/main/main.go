package main

import (
	"fmt"
	"log"
	"net"
	"os"
	"KG/session"
)

func main() {
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

		// session := New(conn)
		// go session.Handler()
	}
}
