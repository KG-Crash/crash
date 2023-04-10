@ECHO OFF

PUSHD src\lobby\main
go get
go build
START main.exe
POPD