@echo off

"%systemroot%\system32\openfiles.exe" > NUL 2>&1
if %errorlevel% neq 0 (
	echo �Ǘ��Ҍ����ŋN������K�v������܂�
	pause
	exit /B 0
)

set m=%1
if "%m%"=="" (
	echo ���[�h�I�� 1:�֘A�t����o�^, 2:�֘A�t��������
	set /p m=
)

pushd %~dp0..\..\
set YMMDir=%CD%
popd

if "%m%"=="1" (
	"%systemroot%\system32\reg.exe" add "HKEY_CLASSES_ROOT\.ymmp" /d YukkuriMovieMaker4.Project /f
	"%systemroot%\system32\reg.exe" add "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project" /d "�������MovieMaker4 �v���W�F�N�g�t�@�C��" /f
	"%systemroot%\system32\reg.exe" add "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project\DefaultIcon" /d "%YMMDir%\Resources\YMMP_logo.ico" /f
	"%systemroot%\system32\reg.exe" add "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project\shell\open\command" /d "%YMMDir%\YukkuriMovieMaker.exe ""%%1""" /f
) else if "%m%"=="2" (
	"%systemroot%\system32\reg.exe" delete "HKEY_CLASSES_ROOT\.ymmp" /f
	"%systemroot%\system32\reg.exe" delete "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project" /f
	"%systemroot%\system32\reg.exe" delete "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project\DefaultIcon" /f
	"%systemroot%\system32\reg.exe" delete "HKEY_CLASSES_ROOT\YukkuriMovieMaker4.Project\shell\open\command" /f
) else (
	echo ���͂��ꂽ�l���s���ł�
	pause
)