@echo off
set "PROJECT_DIR=%~dp0"
taskkill /F /IM WorkItems.Api.exe >nul 2>&1
start "Work Items API" cmd /k "cd /d "%PROJECT_DIR%" && dotnet run --project src\WorkItems.Api --urls http://127.0.0.1:5062"
start "Work Items UI" cmd /k "cd /d "%PROJECT_DIR%src\work-items-ui" && npm.cmd start"
echo Starting the API and Angular UI in separate windows...
echo Open http://localhost:4200 after both windows report they are ready.
