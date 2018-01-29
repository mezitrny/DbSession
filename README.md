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

## DbParameter
SQL parameter wrapper used to isolate concrete provider classes. It expects name, C# type and value

<code>
var parameter = new DbParameter("Id", typeof(int), 5)
  // there is other way of creating parameter instance
var otherParameter = new DbParameter<int>("Id", 5)
</code>

They are commonly aggregated to sets - any parameter set must contain parameter for each placeholder (e.g. parameter with name "Something" if there is placeholder "@Something" in a SQL script).

<code>
var parameterSet = new DbParameterSet
{
  new SqlParameter<string>("Name", "Max"), 
  new SqlParameter<string>("Surname", "Example"), 
  new SqlParameter<int>("Age", 24)
}
</code>

In case of executing batches there are IEnumerable<ParameterSet> to be inserted. The thing is that the same SQL script will be ran once with each parameter set in this enumeration (i.e. with different parameter values each time).
  
<code>
var parameterSetBatch = new DbParameterSet[] 
{ 
  new DbParameterSet
  {
    new SqlParameter<string>("Name", "Max"), 
    new SqlParameter<string>("Surname", "Example"), 
    new SqlParameter<int>("Age", 24)
  },
  new DbParameterSet
  {
    new SqlParameter<string>("Name", "Max"), 
    new SqlParameter<string>("Surname", "Example"), 
    new SqlParameter<int>("Age", 24)
  }
}
</code>

# DbSession.Sqlite [![NuGet](https://img.shields.io/nuget/v/MeziTrny.DbSession.Sqlite.svg)](https://www.nuget.org/packages/MeziTrny.DbSession.Sqlite/)
