using NUnit.Framework;

namespace RoseByte.AdoSession.Tests
{
    [TestFixture]
    public class ParameterTests
    {
        [Test]
        public void ShouldConstructInstance()
        {
            var sut = new Parameter<int>("A", 2);

            Assert.That(sut.Value, Is.EqualTo(2));
            Assert.That(sut.Name, Is.EqualTo("A"));
            Assert.That(sut.Type, Is.EqualTo(typeof(int)));
        }
    }
}