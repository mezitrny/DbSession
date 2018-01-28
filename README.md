# DbSession.Core [![NuGet](https://img.shields.io/nuget/v/MeziTrny.DbSession.Core.svg)](https://www.nuget.org/packages/MeziTrny.DbSession.Core/)

ADO wrapper SQL database session

## DbSession
Facade class publishing all the interesting methods. Its' constructed with connection string of type "Data Source=...".

### DbSession.Select

### DbSession.Execute

### DbSession.GetScalar

### DbSession.ExecuteOnTransaction

### DbSession.Commit

### DbSession.Rollback

### DbSession.CloseConnection

## IValueSet
Fetched row data wrapper used to get rid of DataReader.

## SqlParameter
SQL parameter wrapper used to isolate concrete provider classes. It expects name, C# type and value

<code>
var parameter = new SqlParameter("Id", typeof(int), 5)
</code>

# DbSession.Sqlite [![NuGet](https://img.shields.io/nuget/v/MeziTrny.DbSession.Sqlite.svg)](https://www.nuget.org/packages/MeziTrny.DbSession.Sqlite/)
