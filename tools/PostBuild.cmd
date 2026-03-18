@echo off
setlocal enableextensions
set "SOLROOT=%~1"
if "%SOLROOT%"=="" set "SOLROOT=%~dp0.."
if "%SOLROOT:~-1%"=="\" set "SOLROOT=%SOLROOT:~0,-1%"

rem --- Force Git on PATH for this build only ---
set "PATH=C:\Program Files\Git\cmd;C:\Program Files\Git\mingw64\bin;%PATH%"

for /f "usebackq delims=" %%G in (`where git 2^>nul`) do (
  if not defined GIT_EXE set "GIT_EXE=%%~fG"
)
if not defined GIT_EXE (
  for %%P in (
    "%ProgramFiles%\Git\cmd\git.exe"
    "%ProgramFiles%\Git\bin\git.exe"
    "%ProgramFiles(x86)%\Git\cmd\git.exe"
    "%ProgramFiles(x86)%\Git\bin\git.exe"
    "%ProgramFiles%\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Git\cmd\git.exe"
    "%ProgramFiles%\Microsoft Visual Studio\2022\Professional\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Git\cmd\git.exe"
    "%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Git\cmd\git.exe"
  ) do (
    if exist "%%~fP" set "GIT_EXE=%%~fP"
  )
)
if defined GIT_EXE (
  for %%D in ("%GIT_EXE%") do set "GIT_DIR=%%~dpD"
  set "PATH=%GIT_DIR%;%PATH%"
  echo [PostBuild] Using git: %GIT_EXE%
) else (
  echo [PostBuild] git.exe not found; context.md will omit diffs.
)

rem --- 1) Bump version ---
powershell -NoProfile -ExecutionPolicy Bypass -File "%SOLROOT%\tools\Update-Version.ps1" -Root "%SOLROOT%"
if errorlevel 1 goto :err

rem --- 2) Export VB to txt ---
powershell -NoProfile -ExecutionPolicy Bypass -File "%SOLROOT%\tools\ExportAllVbAsTxt.ps1" -Root "%SOLROOT%" -Output "%SOLROOT%\ChatExports\latest"
if errorlevel 1 goto :err

rem --- 3) Update context (NoDB) ---
powershell -NoProfile -ExecutionPolicy Bypass -File "%SOLROOT%\tools\Update-Context-NoDB.ps1" ^
  -RootDir "%SOLROOT%" ^
  -OutputFile "%SOLROOT%\ChatExports\context.md"
if errorlevel 1 goto :err


REM ============================================================
REM ---4) Export full database single-file script
REM ============================================================
echo [PostBuild] Exporting full database into single .sql file...
pwsh -NoLogo -NoProfile -ExecutionPolicy Bypass ^
  -File "%SOLROOT%\tools\Export-DatabaseSingle.ps1" -Root "%SOLROOT%" ^
  -ServerInstance "." ^
  -DatabaseName "FitnessAndDiet" ^
  -Output "%SOLROOT%\ChatExports\FitnessAndDiet_All.sql"
if errorlevel 1 goto :fail


echo [PostBuild] OK
exit /b 0

:err
echo [PostBuild] FAILED with code %errorlevel%
exit /b %errorlevel%
