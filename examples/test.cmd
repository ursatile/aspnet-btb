
@echo off
echo TEST ALL THE THINGS
for /r %%1 in (*.sln) do dotnet test "%%~f1"
