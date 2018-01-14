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
    }
}