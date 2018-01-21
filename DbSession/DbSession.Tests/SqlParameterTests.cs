using DbSession.Parameters;
using NUnit.Framework;

namespace DbSession.Tests
{
    [TestFixture]
    public class SqlParameterTests
    {
        [Test]
        public void ShouldConstructInstance()
        {
            var sut = new DbParameter<int>("A", 2);

            Assert.That(sut.Value, Is.EqualTo(2));
            Assert.That(sut.Name, Is.EqualTo("A"));
            Assert.That(sut.Type, Is.EqualTo(typeof(int)));
        }
    }
}