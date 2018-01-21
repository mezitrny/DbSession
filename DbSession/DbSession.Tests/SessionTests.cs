using System;
using System.IO;
using DbSession.Connections;
using Moq;
using NUnit.Framework;

namespace DbSession.Tests
{
    [TestFixture]
    public class SessionTests
    {
        [Test]
        public void ShouldConstructClass()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");
            sut.Execute("A");

            factory.Verify(x => x.Create("A"));
            connection.Verify(x => x.Execute("A", null));
        }

        [Test]
        public void ShouldCloseUnusedConnection()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");
            sut.CloseConnection();

            connection.Verify(x => x.Dispose(), Times.Never);
        }

        [Test]
        public void ShouldCloseConnection()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");
            sut.Execute("A");
            sut.CloseConnection();

            connection.Verify(x => x.Execute("A", null));
            connection.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void ShouldDisposeUsedConnection()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");
            sut.Execute("A");
            sut.Dispose();

            connection.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void ShouldNotDisposeUnusedConnection()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");
            sut.Dispose();

            connection.Verify(x => x.Dispose(), Times.Never);
        }

        [Test]
        public void ShouldThrowIfResourceNotFound()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");

            Assert.That(
                () => sut.ReadEmbedded("DbSession.Tests.Sql.Nothing.sql"), 
                Throws.ArgumentException.With.Message.EqualTo("Resource script 'DbSession.Tests.Sql.Nothing.sql' couldn't be found."));
        }

        [Test]
        public void ShouldExecuteFromResource()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");
            Assert.That(sut.ReadEmbedded("DbSession.Tests.Sql.TestScript.sql"), Is.EqualTo("SELECT 1;"));
        }

        [Test]
        public void ShouldThrowIfResourceFileNotFound()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");

            Assert.That(
                () => sut.ReadResource("DbSession.Tests.Sql.Nothing.resx", "key"),
                Throws.ArgumentException.With.Message.EqualTo("Resource file 'DbSession.Tests.Sql.Nothing.resx' couldn't be found."));
        }

        [Test]
        public void ShouldThrowIfKeyInResourceFileNotFound()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");

            Assert.That(
                () => sut.ReadResource("DbSession.Tests.Sql.TestResource", "MissingKey"),
                Throws.ArgumentException);
        }

        [Test]
        public void ShouldExecuteFromResourceFile()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");
            Assert.That(sut.ReadResource("DbSession.Tests.Sql.TestResource", "TestKey"), Is.EqualTo("SELECT 1;"));
        }

        [Test]
        public void ShouldThrowIfFileNotFound()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);

            var sut = new Session(factory.Object, "A");

            Assert.That(
                () => sut.ReadFile("C:\\NotInHere.ttt"),
                Throws.ArgumentException.With.Message.EqualTo("File 'C:\\NotInHere.ttt' couldn't be found."));
        }

        [Test]
        public void ShouldExecuteFromFile()
        {
            var factory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            factory.Setup(x => x.Create("A")).Returns(connection.Object);
            var myFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var path = Path.Combine(myFolder ?? "", "Sql", "TestFile.txt");
            
            var sut = new Session(factory.Object, "A");

            Assert.That(sut.ReadFile(path), Is.EqualTo("SELECT 2;"));
        }
    }
}