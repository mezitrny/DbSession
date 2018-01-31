Push-Location -Path ".\AdoSession"
nuget pack AdoSession.nuspec
Pop-Location

Push-Location -Path ".\AdoSession.Sqlite"
nuget pack AdoSession.Sqlite.nuspec
Pop-Location