using System.Collections.Generic;
using System.Data;
using Moq;
using NUnit.Framework;
using RoseByte.AdoSession.Internals;

namespace RoseByte.AdoSession.Tests
{
    [TestFixture]
    public class ValueSetTests
    {
        [Test]
        public void ShouldReturnString()
        {
            var reader = new Mock<IDataReader>();
            reader.Setup(x => x.FieldCount).Returns(3);
            reader.Setup(x => x.GetName(0)).Returns("string");
            reader.Setup(x => x.GetName(1)).Returns("string?");
            reader.Setup(x => x.GetName(2)).Returns("string?Null");
            reader.Setup(x => x[0]).Returns("A");
            reader.Setup(x => x[1]).Returns("B");
            reader.Setup(x => x[2]).Returns(null);

            var sut = new ValueSet(reader.Object);

            Assert.That(sut.Get<string>("string"), Is.EqualTo("A"));
            Assert.That(sut.Get<string>("string?"), Is.EqualTo("B"));
            Assert.That(sut.Get<string>("string?Null"), Is.Null);
        }

        [Test]
        public void ShouldReturnDictionary()
        {
            var sut = new ValueSet(new Dictionary<string, object>{{ "A", 1 }, { "B", 2 } });

            Assert.That(sut.Values.Count, Is.EqualTo(2));
            Assert.That(sut.Values["A"], Is.EqualTo(1));
            Assert.That(sut.Values["B"], Is.EqualTo(2));
        }
    }
}