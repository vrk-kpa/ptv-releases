@echo off

SET dotnet="C:\Program Files\dotnet\dotnet.exe"
SET opencover="%LOCALAPPDATA%\Apps\OpenCover\OpenCover.Console.exe"
SET reportgenerator="%LOCALAPPDATA%\Apps\OpenCover\ReportGenerator\ReportGenerator.exe"

REM simple checks that the needed executables exist
if not exist %dotnet% (
	echo Missing %dotnet%
	exit /b
)

if not exist %opencover% (
	echo Missing %opencover%
	exit /b
)

if not exist %reportgenerator% (
	echo Missing %reportgenerator%
	exit /b
)

REM the directory where to we create the xml report files from unit tests
SET coverageoutputdir=CoverageOutput

REM the directory where the report is generated
SET coveragereportdir=CoverageReport

if not exist %coverageoutputdir% (
	echo Creating coverage output directory: %coverageoutputdir%
	md %coverageoutputdir%
)

rem can't exclude tests assemblies because then we wont get the what test covered what menu
rem SET filter="+[*]PTV.* -[*.Tests]* -[xunit.*]* -[FluentValidation]*"
SET filter="+[PTV.*]* -[xunit.*]* -[FluentValidation]*"

REM These variables will be changed for each test project the -c DebugCodeCoverageWin use a custom configuration to get full windows debug debugtype=full
SET targetargs="test PTV.ExternalSources.Tests\PTV.ExternalSources.Tests.csproj -c DebugCodeCoverageWin"
SET coveragefile=PTV.ExternalSources.Tests-Coverage.xml

REM ***
REM Run code coverage analysis for our test projects
REM ***

REM PTV.ExternalSources.Tests
%opencover% -oldStyle -register:user -target:%dotnet% -output:"%coverageoutputdir%\%coveragefile%" -targetargs:%targetargs% -filter:%filter% -skipautoprops -hideskipped:All -coverbytest:*.Tests.dll

REM PTV.Framework.Tests
SET targetargs="test PTV.Framework.Tests\PTV.Framework.Tests.csproj -c DebugCodeCoverageWin"
SET coveragefile=PTV.Framework.Tests-Coverage.xml
%opencover% -oldStyle -register:user -target:%dotnet% -output:"%coverageoutputdir%\%coveragefile%" -targetargs:%targetargs% -filter:%filter% -skipautoprops -hideskipped:All -coverbytest:*.Tests.dll

REM PTV.Domain.Model.Tests
SET targetargs="test PTV.Domain.Model.Tests\PTV.Domain.Model.Tests.csproj -c DebugCodeCoverageWin"
SET coveragefile=PTV.Domain.Model.Tests-Coverage.xml
%opencover% -oldStyle -register:user -target:%dotnet% -output:"%coverageoutputdir%\%coveragefile%" -targetargs:%targetargs% -filter:%filter% -skipautoprops -hideskipped:All -coverbytest:*.Tests.dll

REM PTV.Domain.Logic.Tests
SET targetargs="test PTV.Domain.Logic.Tests\PTV.Domain.Logic.Tests.csproj -c DebugCodeCoverageWin"
SET coveragefile=PTV.Domain.Logic.Tests-Coverage.xml
%opencover% -oldStyle -register:user -target:%dotnet% -output:"%coverageoutputdir%\%coveragefile%" -targetargs:%targetargs% -filter:%filter% -skipautoprops -hideskipped:All -coverbytest:*.Tests.dll

REM PTV.Database.Model.Tests
SET targetargs="test PTV.Database.Model.Tests\PTV.Database.Model.Tests.csproj -c DebugCodeCoverageWin"
SET coveragefile=PTV.Database.Model.Tests-Coverage.xml
%opencover% -oldStyle -register:user -target:%dotnet% -output:"%coverageoutputdir%\%coveragefile%" -targetargs:%targetargs% -filter:%filter% -skipautoprops -hideskipped:All -coverbytest:*.Tests.dll

REM PTV.Database.DataAccess.Tests
SET targetargs="test PTV.Database.DataAccess.Tests\PTV.Database.DataAccess.Tests.csproj -c DebugCodeCoverageWin"
SET coveragefile=PTV.Database.DataAccess.Tests-Coverage.xml
%opencover% -oldStyle -register:user -target:%dotnet% -output:"%coverageoutputdir%\%coveragefile%" -targetargs:%targetargs% -filter:%filter% -skipautoprops -hideskipped:All -coverbytest:*.Tests.dll


REM Generate the report from all of the xml files created by opencover
%reportgenerator% -targetdir:%coveragereportdir% -reporttypes:Html;Badges -reports:"%coverageoutputdir%\*.xml" -verbosity:Error

REM Open the report  
start "report" "%coveragereportdir%\index.htm"