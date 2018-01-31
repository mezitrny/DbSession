using System.Data.SQLite;
using System.Linq;
using DbSession.Parameters;
using NUnit.Framework;

namespace Mezitrny.DbSession.Sqlite.Tests
{
    [TestFixture]
    public class SqliteSessionTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE IF NOT EXISTS TestTable(Id INT PRIMARY KEY, TestValue INT);\r\n" +
                                      "DELETE FROM TestTable;\r\n" +
                                      "INSERT INTO TestTable VALUES (1, 5);\r\n" +
                                      "INSERT INTO TestTable VALUES (2, 6);\r\n";
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [Test]
        public void ShouldSelect()
        {
            var sut = new SqliteSession("Data Source=TestDatabase.sqlite");
            var result = sut.Select(
                "SELECT * FROM TestTable WHERE Id IN (@Id1, @Id2)",
                new DbParameterSet { new DbParameter<int>("Id1", 1), new DbParameter<int>("Id2", 2) }).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0]["Id"], Is.EqualTo(1));
            Assert.That(result[0]["TestValue"], Is.EqualTo(5));
            Assert.That(result[1]["Id"], Is.EqualTo(2));
            Assert.That(result[1]["TestValue"], Is.EqualTo(6));
        }

        [Test]
        public void ShouldExecute()
        {
            var sut = new SqliteSession("Data Source=TestDatabase.sqlite");

            sut.Execute("INSERT INTO TestTable VALUES(5, @Value)", new DbParameterSet { new DbParameter<int>("Value", 7) });

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM TestTable WHERE Id = 5";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(7));
            }
        }

    }
}