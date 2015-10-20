using NUnit.Framework;
using EscherAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscherAuthTests
{
    [TestFixture()]
    public class EscherTests
    {
        [Test()]
        public void HelloTest()
        {
            Assert.AreEqual("Hello World", new Escher().Hello());
        }
    }
}
