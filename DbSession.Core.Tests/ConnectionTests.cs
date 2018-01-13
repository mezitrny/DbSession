using System.Data;
using System.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace DbSession.Core.Tests
{
    [TestFixture]
    public class ConnectionTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            using (var connection = new SqlConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "IF NOT (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='TestTable'))\r\n" + 
                                      "CREATE TABLE TestTable(Id INT PRIMARY KEY, TestValue INT)\r\n" +
                                      "ELSE \r\n DELETE FROM TestTable\r\n" +
                                      "INSERT INTO TestTable VALUES (1, 5)\r\n" +
                                      "INSERT INTO TestTable VALUES (2, 6)\r\n";
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            using (var connection = new SqlConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM TestTable";
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [Test]
        public void ShouldExecuteOnTransaction()
        {
            var sut = new Connection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            sut.ExecuteOnTransaction("INSERT INTO TestTable VALUES(3, @Value)", new SqlParameterSet{new SqlParameter<int>("Value", 7)});
            sut.Commit();

            using (var connection = new SqlConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;"))
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
            var sut = new Connection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            sut.ExecuteOnTransaction("INSERT INTO TestTable VALUES(4, @Value)", new SqlParameterSet { new SqlParameter<int>("Value", 7) });
            sut.RollBack();

            using (var connection = new SqlConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;"))
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
            var sut = new Connection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");
            sut.Execute("SELECT * FROM TestTable WHERE Id IN (@Id1, @Id2)", new SqlParameterSet { new SqlParameter<int>("Id1", 1), new SqlParameter<int>("Id2", 2) });


        }

        [Test]
        public void ShouldExecute()
        {
            var sut = new Connection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            sut.Execute("INSERT INTO TestTable VALUES(5, @Value)", new SqlParameterSet { new SqlParameter<int>("Value", 7) });

            using (var connection = new SqlConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;"))
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
            var sut = new Connection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            Assert.That(sut.GetScalar("SELECT TestValue FROM TestTable WHERE Id = 1"), Is.EqualTo(5));
        }

        [Test]
        public void ShouldReturnNullScalarOnMissingValue()
        {
            var sut = new Connection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            Assert.That(sut.GetScalar("SELECT TestValue FROM TestTable WHERE Id = 100"), Is.Null);
        }
    }
}