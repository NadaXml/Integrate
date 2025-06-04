set WORKSPACE=..
set FLATC=%WORKSPACE%\Tools\flatc\flatc.exe
set OUPUT_DIR=output\cs
set SCHEMA_FILE=output\Gen\schema.fbs
set DATA_DIR=output\bytes
set OUTPUT_DATA=%WORKSPACE%\Assets\SRSandBox\Data\Config
set OUTPUT_CS=%WORKSPACE%\Packages\game_adt

rem gen csharp code
@REM %FLATC% -I schemas --gen-object-api --csharp -o %OUPUT_DIR% %SCHEMA_FILE%

%FLATC% -I schemas --csharp -o %OUTPUT_CS% %SCHEMA_FILE%

rem json to bin
%FLATC% -o %OUTPUT_DATA% --binary %SCHEMA_FILE% --root-type cfg.Tbrole %DATA_DIR%\tbrole.json
%FLATC% -o %OUTPUT_DATA% --binary %SCHEMA_FILE% --root-type cfg.Tbsetting %DATA_DIR%\tbsetting.json