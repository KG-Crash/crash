FROM golang:alpine
  
ENV GOPATH=/app
ENV GO111MODULE=off

RUN apk add git

WORKDIR /app
COPY . .
WORKDIR src/main

RUN go get -u gorm.io/gorm
RUN go get -u gorm.io/driver/mysql
RUN go get
RUN go build -o ./main

CMD ./main