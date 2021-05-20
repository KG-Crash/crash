@ECHO OFF

PUSHD %~dp0
SETX GOPATH %CD%
CALL go env -w GO111MODULE=off

PUSHD src\main
go get
go build
CALL main.exe
POPD
POPD
PAUSE