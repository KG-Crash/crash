@ECHO OFF

PUSHD converter
RMDIR output /s /q
CALL app.exe --dir=.. --out=output

PUSHD output
@REM gofmt -s -w class\server\Table.go
@REM gofmt -s -w const\server\Const.go
@REM gofmt -s -w enum\server\Enum.go

@REM RMDIR /s /q ..\..\..\server\src\main\json
@REM ROBOCOPY json\server ..\..\..\server\src\main\json /E /NFL /NDL /NJH /NJS /nc /ns /np

@REM RMDIR /s /q ..\..\..\server\src\Table
@REM ROBOCOPY class\server ..\..\..\server\src\table /E /NFL /NDL /NJH /NJS /nc /ns /np

@REM RMDIR /s /q ..\..\..\server\src\constant
@REM ROBOCOPY const\server ..\..\..\server\src\constant /E /NFL /NDL /NJH /NJS /nc /ns /np

@REM RMDIR /s /q ..\..\..\server\src\enum
@REM ROBOCOPY enum\server ..\..\..\server\src\enum /E /NFL /NDL /NJH /NJS /nc /ns /np

RMDIR /s /q ..\..\..\client\unity\Assets\Shared\Table
ROBOCOPY class\cs\client ..\..\..\client\unity\Assets\Shared\Table\Client  /E /NFL /NDL /NJH /NJS /nc /ns /np
ROBOCOPY class\cs\common ..\..\..\client\unity\Assets\Shared\Table\Common  /E /NFL /NDL /NJH /NJS /nc /ns /np
COPY bind\cs\client\Table.cs ..\..\..\client\unity\Assets\Shared\Table

RMDIR /s /q ..\..\..\client\unity\Assets\Shared\Enum
ROBOCOPY enum\cs ..\..\..\client\unity\Assets\Shared\Enum  /E /NFL /NDL /NJH /NJS /nc /ns /np

COPY const\cs\client\Const.cs ..\..\..\client\unity\Assets\Shared
RMDIR /s /q ..\..\..\client\unity\json
ROBOCOPY json\client ..\..\..\client\unity\json /E /NFL /NDL /NJH /NJS /nc /ns /np
pause