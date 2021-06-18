REM download ubuntu image
curl http://cdimage.ubuntu.com/ubuntu-base/releases/18.04.1/release/ubuntu-base-18.04.5-base-amd64.tar.gz -J -L -o ubuntu.tar.gz

REM re-install ubuntu instance
LxRunOffline.exe uninstall -n ubuntu
LxRunOffline.exe install -n ubuntu -f ubuntu.tar.gz -d ubuntu

REM delete ubuntu image
del /f ubuntu.tar.gz

REM provision
LxRunOffline.exe run -n ubuntu -c ./provision.sh
LxRunOffline.exe run -n ubuntu