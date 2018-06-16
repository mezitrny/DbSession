using System;
using System.Data;
using System.IO;
using Moq;
using NUnit.Framework;
using RoseByte.AdoSession.Interfaces;
using RoseByte.AdoSession.Internals;

namespace RoseByte.AdoSession.Tests
{
    [TestFixture]
    public class SqlServerSessionTests
    {
        [Test]
        public void ShouldExecuteProcedure()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new SqlServerSession(factory.Object, "A");
            sut.ExecuteProcedure("A");

            connection.Verify(x => x.Execute("A", null, CommandType.StoredProcedure));
        }

        [Test]
        public void ShouldExecuteProcedureBatch()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);
            var parameterSet = new[] {new ParameterSet()};

            var sut = new SqlServerSession(factory.Object, "A");
            sut.ExecuteProcedureBatch("A", parameterSet);

            connection.Verify(x => x.ExecuteBatch("A", parameterSet, CommandType.StoredProcedure));
        }

        [Test]
        public void ShouldExecuteProcedureOnTransaction()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new SqlServerSession(factory.Object, "A");
            sut.ExecuteProcedureOnTransaction("A");

            connection.Verify(x => x.ExecuteOnTransaction("A", null, CommandType.StoredProcedure));
        }
        
        [Test]
        public void ShouldParseDatabaseAndServer()
        {
            var sut = new SqlServerSession("Data Source=myHost;Initial Catalog=myBase;Integrated Security=True");
            Assert.That(sut.Database, Is.EqualTo("myBase"));
            Assert.That(sut.Server, Is.EqualTo("myHost"));
        }

        [Test]
        public void ShouldExecuteProcedureBatchOnTransaction()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);
            var parameterSet = new[] { new ParameterSet() };

            var sut = new SqlServerSession(factory.Object, "A");
            sut.ExecuteProcedureBatchOnTransaction("A", parameterSet);

            connection.Verify(x => x.ExecuteBatchOnTransaction("A", parameterSet, CommandType.StoredProcedure));
        }
    }
}