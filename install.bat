REM Clone folder from Github
echo Clone Github folder > log.txt 2>&1
git clone https://github.com/Ryu204/Expenser >> log.txt 2>&1
if %ERRORLEVEL% neq 0 ( 
  color 4 
  cls 
  echo Cannot clone repository
  pause
  exit \b
)

REM Navigate to project folder
cd Expenser

REM Check if dotnet is available
echo Check dotnet available >> log.txt 2>&1
where dotnet >> log.txt 2>&1
if %ERRORLEVEL% equ 0 (
  dotnet build -c Release -o Release >> log.txt 2>&1
  if %ERRORLEVEL% == 0 ( 
    cd Release
    mkdir Users
    .> Users/userlist.list 2>NUL
    color 2
    cls
    echo Build succeeded 
    ) else ( 
      cls
      color 4
      echo Build failed, check the file log.txt for more information )
) else ( 
cls 
echo dotnet is not available )

pause