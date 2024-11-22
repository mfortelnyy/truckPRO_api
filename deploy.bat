@echo off
echo Starting deployment...

REM Navigate to the repository
cd C:\Users\Administrator\Documents\truckProRepo\truckPRO_api

REM Pull the latest changes from GitHub
git checkout deployment
git pull

REM Stop IIS to release file locks
iisreset /stop
REM Wait for 20 seconds to ensure IIS stops completely
timeout /t 20 /nobreak >nul  

REM Publish the application directly to the IIS root
dotnet publish -c Release -r win-x64 --self-contained true /p:DeleteExistingFiles=true -o C:\inetpub\wwwroot\truckProApi ./truckPRO_api.csproj

REM Start IIS again
iisreset /start

echo Running deploy script > C:\Users\Administrator\Documents\truckProRepo\log.txt

echo Deployment completed successfully!
pause