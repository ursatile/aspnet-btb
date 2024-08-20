
@echo off
echo Running dotnet test on all the things
for /r %%1 in (*.sln) do dotnet test "%%~f1"
