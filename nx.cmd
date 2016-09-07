@echo off
setlocal
set bin=%~dp0\node_modules\.bin
echo ^> %*
"%bin%"\\%*