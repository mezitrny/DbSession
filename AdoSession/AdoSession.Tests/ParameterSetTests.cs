using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RoseByte.AdoSession.Interfaces;

namespace RoseByte.AdoSession.Tests
{
    [TestFixture]
    public class ParameterSetTests
    {
        [Test]
        public void ShouldCreateWithParameters()
        {
            var parameters = new List<IParameter>
            {
                new Mock<IParameter>().Object,
                new Mock<IParameter>().Object,
                new Mock<IParameter>().Object
            };
            
            var sut = new ParameterSet(parameters);
            
            Assert.That(sut.Count, Is.EqualTo(3));
        }
    }
}