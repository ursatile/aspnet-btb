# Define the root path where the script should start looking for .sln files
$rootPath = Get-Location

# Get all directories containing a .sln file
$solutionDirectories = Get-ChildItem -Path $rootPath -Recurse -Filter Rockaway.sln | Select-Object -ExpandProperty DirectoryName | Sort-Object -Unique

# Loop through each directory and execute dotnet commands
foreach ($dir in $solutionDirectories) {
	Write-Host "Processing directory: $dir"

	# Change to the directory containing the .sln file
	Set-Location -Path $dir

	# Run dotnet clean, build, and test
	Write-Host "Running dotnet clean..."
	dotnet clean --verbosity quiet
	Remove-Item -Recurse Rockaway.WebApp/bin
	Remove-Item -Recurse Rockaway.WebApp/obj
	Remove-Item -Recurse Rockaway.WebApp.Tests/bin
	Remove-Item -Recurse Rockaway.WebApp.Tests/obj

	Write-Host "Running dotnet build..."
	dotnet build

	Write-Host "Running dotnet test..."
	dotnet test

	# Return to the root path before processing the next directory
	Set-Location -Path $rootPath
}

Write-Host "Done."
