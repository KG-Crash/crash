@ECHO OFF

PUSHD converter
RMDIR output /s /q
CALL app.exe --dir=.. --out=output

PUSHD output
gofmt -s -w class\server\Table.go
gofmt -s -w const\server\Const.go
gofmt -s -w enum\server\Enum.go

RMDIR /s /q ..\..\..\server\src\main\json
ROBOCOPY json\server ..\..\..\server\src\main\json /E /NFL /NDL /NJH /NJS /nc /ns /np

RMDIR /s /q ..\..\..\server\src\Table
ROBOCOPY class\server ..\..\..\server\src\table /E /NFL /NDL /NJH /NJS /nc /ns /np

RMDIR /s /q ..\..\..\server\src\constant
ROBOCOPY const\server ..\..\..\server\src\constant /E /NFL /NDL /NJH /NJS /nc /ns /np

RMDIR /s /q ..\..\..\server\src\enum
ROBOCOPY enum\server ..\..\..\server\src\enum /E /NFL /NDL /NJH /NJS /nc /ns /np

COPY class\client\Table.cs ..\..\..\client\unity\Assets\Shared
COPY enum\client\Enum.cs ..\..\..\client\unity\Assets\Shared
COPY const\client\Const.cs ..\..\..\client\unity\Assets\Shared
RMDIR /s /q ..\..\..\client\unity\json
ROBOCOPY json\client ..\..\..\client\unity\json /E /NFL /NDL /NJH /NJS /nc /ns /np