set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t all ^
    -c flatbuffers ^
	-d flatbuffers-json	  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=output/bytes ^
	-x outputCodeDir=output/Gen

pause