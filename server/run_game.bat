@ECHO OFF

PUSHD src\game\main
go get
go build
START main.exe
POPD