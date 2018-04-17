using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using RoseByte.AdoSession.SqlServer;

namespace RoseByte.AdoSession.Tests
{
    [TestFixture]
    public class SqlServerConnectionTests
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
                command.CommandText = "IF  EXISTS " +
                                      "  (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestProc]') " +
                                      "  AND type in (N'P', N'PC')) " +
                                      "DROP PROCEDURE [dbo].[TestProc]";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE PROCEDURE dbo.TestProc @ParOne INT, @ParTwo INT AS BEGIN \r\n" +
                                      "INSERT INTO TestTable VALUES (@ParOne, @ParTwo) \r\n END";
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
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            sut.ExecuteOnTransaction("INSERT INTO TestTable VALUES(3, @Value)", new ParameterSet{new Parameter<int>("Value", 7)});
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
        public void ShouldExecuteProcedureOnTransaction()
        {
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            sut.ExecuteOnTransaction(
                "dbo.TestProc", 
                new ParameterSet{new Parameter<int>("ParOne", 100), new Parameter<int>("ParTwo", 101)}, 
                CommandType.StoredProcedure);
            
            sut.Commit();

            using (var connection = new SqlConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM TestTable WHERE Id = 100";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(101));
            }
        }

        [Test]
        public void ShouldExecuteOnTransactionForMultipleSets()
        {
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            sut.ExecuteBatchOnTransaction("INSERT INTO TestTable VALUES(@Id, @Value)", new []
            {
                new ParameterSet { new Parameter<int>("Id", 77), new Parameter<int>("Value", 77) },
                new ParameterSet { new Parameter<int>("Id", 78), new Parameter<int>("Value", 78) }
            });
            sut.Commit();

            using (var connection = new SqlConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM TestTable WHERE Id = 77";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(77));

                command.CommandText = "SELECT TestValue FROM TestTable WHERE Id = 78";
                result = int.Parse(command.ExecuteScalar().ToString());
                Assert.That(result, Is.EqualTo(78));
            }
        }

        [Test]
        public void ShouldRollbackTransaction()
        {
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            sut.ExecuteOnTransaction("INSERT INTO TestTable VALUES(4, @Value)", new ParameterSet { new Parameter<int>("Value", 7) });
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
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");
            var result = sut.Select(
                "SELECT * FROM TestTable WHERE Id IN (@Id1, @Id2)", 
                new ParameterSet { new Parameter<int>("Id1", 1), new Parameter<int>("Id2", 2) }).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0]["Id"], Is.EqualTo(1));
            Assert.That(result[0]["TestValue"], Is.EqualTo(5));
            Assert.That(result[1]["Id"], Is.EqualTo(2));
            Assert.That(result[1]["TestValue"], Is.EqualTo(6));
        }

        [Test]
        public void ShouldReleaseDatasetAfterSelect()
        {
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");
            var result = sut.Select(
                "SELECT * FROM TestTable WHERE Id IN (@Id1, @Id2)",
                new ParameterSet { new Parameter<int>("Id1", 1), new Parameter<int>("Id2", 2) });
            var result2 = sut.Select(
                "SELECT * FROM TestTable WHERE Id IN (@Id1, @Id2)",
                new ParameterSet { new Parameter<int>("Id1", 1), new Parameter<int>("Id2", 2) });

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result2.Count(), Is.EqualTo(2));
        }

        [Test]
        public void ShouldExecuteBatch()
        {
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            sut.ExecuteBatch("INSERT INTO TestTable VALUES(@Id, @Value)", new []
            {
                new ParameterSet { new Parameter<int>("Id", 79), new Parameter<int>("Value", 79) } ,
                new ParameterSet { new Parameter<int>("Id", 80), new Parameter<int>("Value", 80) }
            });

            using (var connection = new SqlConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT TestValue FROM TestTable WHERE Id = 79";
                connection.Open();
                var result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(79));

                command.CommandText = "SELECT TestValue FROM TestTable WHERE Id = 80";
                result = int.Parse(command.ExecuteScalar().ToString());

                Assert.That(result, Is.EqualTo(80));
            }
        }

        [Test]
        public void ShouldExecute()
        {
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            sut.Execute("INSERT INTO TestTable VALUES(5, @Value)", new ParameterSet { new Parameter<int>("Value", 7) });

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
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            Assert.That(sut.GetScalar("SELECT TestValue FROM TestTable WHERE Id = 1"), Is.EqualTo(5));
        }

        [Test]
        public void ShouldReturnNullScalarOnMissingValue()
        {
            var sut = new SqlServerConnection("Data Source=.;Initial Catalog=DbSessionTests;Integrated Security=True;");

            Assert.That(sut.GetScalar("SELECT TestValue FROM TestTable WHERE Id = 100"), Is.Null);
        }
    }
}