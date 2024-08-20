# Define the base directory where the projects are located

# Get all the project directories
Push-Location
$projects  = Get-ChildItem  -Recurse -File

foreach ($file in $projects) {
	if ($file.FullName -match 'examples\\(\d{3})\\Rockaway\\Rockaway.WebApp\\Rockaway.WebApp.csproj$') {
		$dir = "$($file.DirectoryName)\Migrations"
		Write-Output $dir
		if ($previous) {
			Write-Output "robocopy /MIR $($previous) $dir"
		}
		$previous = $dir
		Write-Output $Matches[1]
		cd $file.DirectoryName
		dotnet ef migrations has-pending-model-changes
	}
    # # Extract the numeric part of the folder path
    # if ($csproj -match '(\d+)') {
    #     $numericPart = $matches[1]
    #     # Add the project to the solution file with the -s flag
    #     dotnet sln examples.sln add $csproj -s $numericPart
	# 	if ($numericPart -ge 500) {
	# 		dotnet sln examples-500.sln add $csproj -s $numericPart
	# 		Write-Host $csproj
	# 	}
    # }
}
Pop-Location
# Output a message when done
Write-Host "Migrations recreated"
