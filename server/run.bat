@ECHO OFF

CALL go env -w GO111MODULE=off

PUSHD src\game\main
go get
go build
START main.exe
POPD

PUSHD src\lobby\main
go get
go build
START main.exe
POPD
PAUSE