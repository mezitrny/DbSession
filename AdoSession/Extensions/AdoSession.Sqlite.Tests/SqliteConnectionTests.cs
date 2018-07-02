using System.Data.SQLite;
using System.Linq;
using NUnit.Framework;

namespace RoseByte.AdoSession.Sqlite.Tests
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
                command.CommandText = "DROP TABLE IF EXISTS SqliteTestTableOne ;\r\n" +
                                      "CREATE TABLE IF NOT EXISTS SqliteTestTableOne(Id INT PRIMARY KEY, TestValue INT);\r\n" +
                                      "DELETE FROM SqliteTestTableOne;\r\n" +
                                      "INSERT INTO SqliteTestTableOne VALUES (1, 5);\r\n" +
                                      "INSERT INTO SqliteTestTableOne VALUES (2, 6);\r\n";
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [Test]
        public void ShouldExecuteOnTransaction()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            sut.ExecuteOnTransaction("INSERT INTO SqliteTestTableOne VALUES(3, @Value)", new ParameterSet { new Parameter<int>("Value", 7) });
            sut.Commit();

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM SqliteTestTableOne WHERE Id = 3";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(7));
            }
        }

        [Test]
        public void ShouldRollbackTransaction()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            sut.ExecuteOnTransaction("INSERT INTO SqliteTestTableOne VALUES(4, @Value)", new ParameterSet { new Parameter<int>("Value", 7) });
            sut.RollBack();

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM SqliteTestTableOne WHERE Id = 4";
                connection.Open();
                var result = command.ExecuteScalar();

                Assert.That(result, Is.Null);
            }
        }

        [Test]
        public void ShouldSelect()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");
            var result = sut.Select(
                "SELECT * FROM SqliteTestTableOne WHERE Id IN (@Id1, @Id2)",
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
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            sut.Execute("INSERT INTO SqliteTestTableOne VALUES(5, @Value)", new ParameterSet { new Parameter<int>("Value", 7) });

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM SqliteTestTableOne WHERE Id = 5";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(7));
            }
        }

        [Test]
        public void ShouldExecuteOnTransactionForMultipleSets()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            sut.ExecuteBatchOnTransaction("INSERT INTO SqliteTestTableOne VALUES(@Id, @Value)", new[]
            {
                new ParameterSet { new Parameter<int>("Id", 77), new Parameter<int>("Value", 77) },
                new ParameterSet { new Parameter<int>("Id", 78), new Parameter<int>("Value", 78) }
            });
            sut.Commit();

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM SqliteTestTableOne WHERE Id = 77";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(77));

                command.CommandText = "SELECT TestValue FROM SqliteTestTableOne WHERE Id = 78";
                result = int.Parse(command.ExecuteScalar().ToString());
                Assert.That(result, Is.EqualTo(78));
            }
        }

        [Test]
        public void ShouldExecuteBatch()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            sut.ExecuteBatch("INSERT INTO SqliteTestTableOne VALUES(@Id, @Value)", new[]
            {
                new ParameterSet { new Parameter<int>("Id", 79), new Parameter<int>("Value", 79) } ,
                new ParameterSet { new Parameter<int>("Id", 80), new Parameter<int>("Value", 80) }
            });

            using (var connection = new SQLiteConnection("Data Source=TestDatabase.sqlite"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM SqliteTestTableOne WHERE Id = 79";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(79));

                command.CommandText = "SELECT TestValue FROM SqliteTestTableOne WHERE Id = 80";
                result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(80));
            }
        }

        [Test]
        public void ShouldReturnScalar()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            Assert.That(sut.GetScalar("SELECT TestValue FROM SqliteTestTableOne WHERE Id = 1"), Is.EqualTo(5));
        }

        [Test]
        public void ShouldReturnNullScalarOnMissingValue()
        {
            var sut = new SqliteConnection("Data Source=TestDatabase.sqlite");

            Assert.That(sut.GetScalar("SELECT TestValue FROM SqliteTestTableOne WHERE Id = 100"), Is.Null);
        }
    }
}