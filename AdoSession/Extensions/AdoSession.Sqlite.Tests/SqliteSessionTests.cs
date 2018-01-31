using System.Data.SQLite;
using System.Linq;
using NUnit.Framework;

namespace RoseByte.AdoSession.Sqlite.Tests
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
                command.CommandText = "DROP TABLE IF EXISTS SqliteTestTableTwo;\r\n" +
                                      "CREATE TABLE IF NOT EXISTS SqliteTestTableTwo(Id INT PRIMARY KEY, TestValue INT);\r\n" +
                                      "DELETE FROM SqliteTestTableTwo;\r\n" +
                                      "INSERT INTO SqliteTestTableTwo VALUES (1, 5);\r\n" +
                                      "INSERT INTO SqliteTestTableTwo VALUES (2, 6);\r\n";
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [Test]
        public void ShouldSelect()
        {
            var sut = new SqliteSession("Data Source=TestDatabase.sqlite");
            var result = sut.Select(
                "SELECT * FROM SqliteTestTableTwo WHERE Id IN (@Id1, @Id2)",
                new ParameterSet { new Parameter<int>("Id1", 1), new Parameter<int>("Id2", 2) }).ToList();

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

            sut.Execute("INSERT INTO SqliteTestTableTwo VALUES(5, @Value)", new ParameterSet { new Parameter<int>("Value", 7) });

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM SqliteTestTableTwo WHERE Id = 5";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(7));
            }
        }

    }
}