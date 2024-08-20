
@echo off
echo Running dotnet clean on all the things
for /r %%1 in (*.sln) do dotnet clean "%%~f1"
echo CLEAN ALL THE THINGS