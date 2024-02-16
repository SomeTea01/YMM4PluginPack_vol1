@echo off

"%systemroot%\system32\openfiles.exe" > NUL 2>&1
if %errorlevel% neq 0 (
	echo 管理者権限で起動する必要があります
	pause
	exit /B 0
)

set m=%1
if "%m%"=="" (
	echo モード選択 1:関連付けを登録, 2:関連付けを解除
	set /p m=
)

pushd %~dp0..\..\
set YMMDir=%CD%
popd

if "%m%"=="1" (
	"%systemroot%\system32\reg.exe" add "HKEY_CLASSES_ROOT\.ymmp" /d YukkuriMovieMaker4.Project /f
	"%systemroot%\system32\reg.exe" add "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project" /d "ゆっくりMovieMaker4 プロジェクトファイル" /f
	"%systemroot%\system32\reg.exe" add "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project\DefaultIcon" /d "%YMMDir%\Resources\YMMP_logo.ico" /f
	"%systemroot%\system32\reg.exe" add "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project\shell\open\command" /d "%YMMDir%\YukkuriMovieMaker.exe ""%%1""" /f
) else if "%m%"=="2" (
	"%systemroot%\system32\reg.exe" delete "HKEY_CLASSES_ROOT\.ymmp" /f
	"%systemroot%\system32\reg.exe" delete "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project" /f
	"%systemroot%\system32\reg.exe" delete "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project\DefaultIcon" /f
	"%systemroot%\system32\reg.exe" delete "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project\shell\open\command" /f
) else (
	echo 入力された値が不正です
	pause
)