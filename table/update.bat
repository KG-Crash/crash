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
pause

rem RMDIR /s/q ..\..\shared\NetworkShared\MasterDataType
rem ROBOCOPY output\class\common ..\..\shared\NetworkShared\MasterDataType /E /NFL /NDL /NJH /NJS /nc /ns /np

rem RMDIR /s/q ..\..\client\ClientShared\MasterDataType
rem ROBOCOPY output\class\client ..\..\client\ClientShared\MasterDataType /E /NFL /NDL /NJH /NJS /nc /ns /np
rem RMDIR /s/q ..\..\server\ServerShared\MasterDataType
rem ROBOCOPY output\class\server ..\..\server\ServerShared\MasterDataType /E /NFL /NDL /NJH /NJS /nc /ns /np

rem DEL /Q /F ..\..\client\ClientShared\Table\Table.cs
rem COPY output\bind\client\Table.cs ..\..\client\ClientShared\Table\Table.cs >NUL
rem DEL /Q /F ..\..\server\ServerShared\Table\Table.cs
rem COPY output\bind\server\Table.cs ..\..\server\ServerShared\Table\Table.cs >NUL

rem RMDIR /s/q ..\..\client\ClientShared\json
rem ROBOCOPY output\json\client ..\..\client\ClientShared\json /E /NFL /NDL /NJH /NJS /nc /ns /np

rem RMDIR /s/q ..\..\server\ServerShared\json
rem ROBOCOPY output\json\server ..\..\server\ServerShared\json /E /NFL /NDL /NJH /NJS /nc /ns /np

rem RMDIR /s/q ..\..\shared\NetworkShared\Enum
rem ROBOCOPY output\enum ..\..\shared\NetworkShared\Enum /E /NFL /NDL /NJH /NJS /nc /ns /np

rem RMDIR /s/q ..\..\client\ClientShared\Const
rem ROBOCOPY output\const\client ..\..\client\ClientShared\Const /E /NFL /NDL /NJH /NJS /nc /ns /np

rem RMDIR /s/q ..\..\client\ServerShared\Const
rem ROBOCOPY output\const\server ..\..\server\ServerShared\Const /E /NFL /NDL /NJH /NJS /nc /ns /np

rem POPD
rem ROBOCOPY ..\client\UnityClient\Assets\Resources\MapFile ..\server\ServerShared\Resources\Map *.json /E /NFL /NDL /NJH /NJS /nc /ns /np