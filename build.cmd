@echo off

if not exist tools\Cake\Cake.exe ( 
	echo Installing Cake...
	tools\nuget\nuget.exe install Cake -OutputDirectory tools -ExcludeVersion -NonInteractive -NoCache
	echo.
)

SET TARGET="All"
IF NOT [%1]==[] (set TARGET="%1")
SET BUILDMODE="Release"
IF NOT [%2]==[] (set BUILDMODE="%2")

echo Starting Cake...
tools\Cake\Cake.exe build.cake -target=%TARGET% -configuration=%BUILDMODE% -verbosity=verbose

:Quit
exit /b %errorlevel%
