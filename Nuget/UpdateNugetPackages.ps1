Push-Location -Path ".\DbSession"
nuget pack DbSession.nuspec
Pop-Location

Push-Location -Path ".\DbSession.Sqlite"
nuget pack DbSession.Sqlite.nuspec
Pop-Location