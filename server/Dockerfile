FROM golang:alpine
ARG SERVICE
  
ENV GOPATH=/app
ENV GO111MODULE=off

RUN apk add git

WORKDIR /app
COPY . .

WORKDIR src
RUN go get -u gorm.io/gorm
RUN go get -u gorm.io/driver/mysql

WORKDIR ${SERVICE}/main
RUN go get
RUN go build -o ./main

CMD ./main