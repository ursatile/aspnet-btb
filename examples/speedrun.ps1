if ($args[0]) {
	$target = $args[0]
	Write-Host "Speedrunning Rockaway in $target"
	copy speedrun.gitignore $target\.gitignore
	copy speedrun.README.md $target\README.md

	pushd
	cd $target
	git init
	git add .
	git commit -m ".gitignore and README"
	popd

	$modules = Get-ChildItem -file -recurse | Where-Object { $_.Name -eq "Rockaway.sln" }
	foreach ($module in $modules) {
		if ($module -match '(\d\d\d)') {
			$moduleNumber = $matches[1]
			$md = @(gci ..\$moduleNumber-*.md)[0]
			$migration = 0
			if ($md) {
				$migration = Select-String -Pattern "migration: .*" $md -Raw
				if ($migration) {
					$migration = $migration.Substring(11)
					Write-Host $migration
				}
				$summary = Select-String -Pattern "summary: .*" $md -Raw
				if ($summary) {
					$summary = $summary.Substring(10).Trim('"', ' ');
					Write-Host $summary
				}
			}
			#			dotnet clean $module
			xcopy /s /y /exclude:speedrun-exclusions.txt $module.DirectoryName $target
			pushd
			cd $target
			if ($migration) {
				dotnet build
				# dotnet test -c Release
				cd Rockaway.WebApp
				dotnet ef migrations add $migration -- --environment=Staging
				dotnet format
				cd ..
			}
			git add .
			git commit -m "Module ${moduleNumber}: $summary"
			popd
		}
	}
	Write-Host "All done! Yay!"
}
else {
	Write-Host "Usage: speedrun.ps1 <path-to-empty-git-repo>"
}

#

# foreach ($csproj in $projects) {
# 	Write-Host $csproj
#     # Extract the numeric part of the folder path
#     if ($csproj -match '(\d+)') {
#         $numericPart = $matches[1]
#         # Add the project to the solution file with the -s flag
#         dotnet sln examples.sln add $csproj -s $numericPart
# 		if ($numericPart -ge 500) {
# 			dotnet sln examples-500.sln add $csproj -s $numericPart
# 			Write-Host $csproj
# 		}
#     }
# }

# # Output a message when done
# Write-Host "All projects added to the solution file."
