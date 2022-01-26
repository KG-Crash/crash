@ECHO OFF

setlocal enableextensions

REM https://velog.io/@springkim/windows-batch-script
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
if '%errorlevel%' NEQ '0' (
    echo Get admin permission...
    goto UACPrompt
) else ( goto gotAdmin )
:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    set params = %*:"=""
    echo UAC.ShellExecute "cmd.exe", "/c %~s0 %params%", "", "runas", 1 >> "%temp%\getadmin.vbs"
    "%temp%\getadmin.vbs"
    rem del "%temp%\getadmin.vbs"
    exit /B
:gotAdmin

pushd "%CD%"
cd /D "%~dp0"

cd .\client
set /A instance_count=3

for /L %%i in (0,1,%instance_count%) do (
    if not exist .\unity_instance_%%i mkdir .\unity_instance_%%i
    cd .\unity_instance_%%i

    if not exist Assets mklink /D Assets ..\unity\Assets
    if not exist json mklink /D json ..\unity\json
    if not exist Packages mklink /D Packages ..\unity\Packages
    if not exist ProjectSettings mklink /D ProjectSettings ..\unity\ProjectSettings

    cd ../
)