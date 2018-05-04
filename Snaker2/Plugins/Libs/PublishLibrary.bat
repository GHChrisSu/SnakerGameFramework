set CurDir=%~dp0
set LibName=%1

if "%LibName%"=="ILRScript" (
	xcopy %LibName%.dll %CurDir%..\..\Assets\StreamingAssets\ILR\ /y
 ) else (
	xcopy %LibName%.dll %CurDir%..\..\Assets\Plugins\ManagedLib\ /y
)