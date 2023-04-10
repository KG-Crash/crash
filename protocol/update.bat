@ECHO OFF

RMDIR Protocols /s /q
flatc --csharp request.fbs
flatc --csharp response.fbs
flatc --go request.fbs
flatc --go response.fbs

app.exe --root=github.com/KG-Crash/crash/protocol

RMDIR ..\client\unity\Assets\Shared\Protocol\FlatBuffer /s /q
ROBOCOPY FlatBuffer ..\client\unity\Assets\Shared\Protocol\FlatBuffer *.cs /E /njh /njs /ndl /nc /ns


RMDIR ..\client\unity\Assets\Shared\Protocol\Wrap /s /q
ROBOCOPY output\cs ..\client\unity\Assets\Shared\Protocol\Wrap *.cs /E /njh /njs /ndl /nc /ns

RMDIR ..\server\src\protocol\Request /s /q
RMDIR ..\server\src\protocol\Response /s /q
RMDIR ..\server\src\protocol\FlatBuffer /s /q
ROBOCOPY FlatBuffer ..\server\src\protocol\FlatBuffer *.go /E /njh /njs /ndl /nc /ns
ROBOCOPY output\go ..\server\src\protocol *.go /E /njh /njs /ndl /nc /ns

PUSHD ..\server\src\protocol
gofmt -s -w Request\Request.go
gofmt -s -w Response\Response.go
gofmt -s -w Protocol.go
POPD

PUSHD ..\server\src\game\main
go get
POPD

RMDIR FlatBuffer /s /q
RMDIR output /s /q
PAUSE