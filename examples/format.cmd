
@echo off
echo Running dotnet format on all the things
for /r %%1 in (*.sln) do dotnet format "%%~f1"
