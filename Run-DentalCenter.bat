@echo off
cd /d "%~dp0"

if exist "publish\DentalCenter.exe" (
  start "" "%~dp0publish\DentalCenter.exe"
  exit /b 0
)

if exist "bin\Release\net10.0\win-x64\DentalCenter.exe" (
  start "" "%~dp0bin\Release\net10.0\win-x64\DentalCenter.exe"
  exit /b 0
)

if exist "bin\Release\net10.0\DentalCenter.exe" (
  start "" "%~dp0bin\Release\net10.0\DentalCenter.exe"
  exit /b 0
)

if exist "bin\Debug\net10.0\win-x64\DentalCenter.exe" (
  start "" "%~dp0bin\Debug\net10.0\win-x64\DentalCenter.exe"
  exit /b 0
)

if exist "bin\Debug\net10.0\DentalCenter.exe" (
  start "" "%~dp0bin\Debug\net10.0\DentalCenter.exe"
  exit /b 0
)

echo First build or publish the project, then run this file again.
echo.
echo   PowerShell:  .\build-exe.ps1
echo.
pause
