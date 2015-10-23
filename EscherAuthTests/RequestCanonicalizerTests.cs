using NUnit.Framework;
using EscherAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EscherAuthTests.Helpers;

namespace EscherAuthTests
{
    [TestFixture()]
    public class RequestCanonicalizerTests
    {
        [Test()]
        public void CanonicalizeTest()
        {
            var a = TestFixtureReader.Read(@"TestFixtures/aws4_testsuite/signrequest-get-vanilla.json");

            var canonicalizer = new RequestCanonicalizer();
            var canonicalizedRequest = canonicalizer.Canonicalize(a.request);

            Assert.AreEqual("", canonicalizedRequest, "canonicalization does not work");
        }
    }
}
