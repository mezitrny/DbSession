Push-Location -Path ".\AdoSession"
..\nuget.exe pack AdoSession.nuspec
Pop-Location

Push-Location -Path ".\AdoSession.Sqlite"
..\nuget.exe pack AdoSession.Sqlite.nuspec
Pop-Location