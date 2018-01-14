using System.Data.SQLite;
using DbSession.Parameters;
using NUnit.Framework;

namespace DbSession.Sqlite.Tests
{
    [TestFixture]
    public class SqliteConnectionTests
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
        public void ShouldExecuteOnTransaction()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            sut.ExecuteOnTransaction("INSERT INTO TestTable VALUES(3, @Value)", new SqlParameterSet { new SqlParameter<int>("Value", 7) });
            sut.Commit();

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM TestTable WHERE Id = 3";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(7));
            }
        }

        [Test]
        public void ShouldRollbackTransaction()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            sut.ExecuteOnTransaction("INSERT INTO TestTable VALUES(4, @Value)", new SqlParameterSet { new SqlParameter<int>("Value", 7) });
            sut.RollBack();

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM TestTable WHERE Id = 4";
                connection.Open();
                var result = command.ExecuteScalar();

                Assert.That(result, Is.Null);
            }
        }

        [Test]
        public void ShouldSelect()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");
            sut.Execute("SELECT * FROM TestTable WHERE Id IN (@Id1, @Id2)", new SqlParameterSet { new SqlParameter<int>("Id1", 1), new SqlParameter<int>("Id2", 2) });


        }

        [Test]
        public void ShouldExecute()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            sut.Execute("INSERT INTO TestTable VALUES(5, @Value)", new SqlParameterSet { new SqlParameter<int>("Value", 7) });

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM TestTable WHERE Id = 5";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(7));
            }
        }

        [Test]
        public void ShouldReturnScalar()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            Assert.That(sut.GetScalar("SELECT TestValue FROM TestTable WHERE Id = 1"), Is.EqualTo(5));
        }

        [Test]
        public void ShouldReturnNullScalarOnMissingValue()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            Assert.That(sut.GetScalar("SELECT TestValue FROM TestTable WHERE Id = 100"), Is.Null);
        }
    }
}