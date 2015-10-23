using NUnit.Framework;
using EscherAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EscherAuthTests.Helpers;
using System.IO;

namespace EscherAuthTests
{
    [TestFixture()]
    public class RequestCanonicalizerTests
    {
        [Test(), TestCaseSource("TestFixtures")]
        public void CanonicalizeTest(TestFixture testFixture)
        {
            var canonicalizer = new RequestCanonicalizer();
            var canonicalizedRequest = canonicalizer.Canonicalize(testFixture.request, testFixture.headersToSign);

            try
            {
                Assert.AreEqual(testFixture.expected.canonicalizedRequest, canonicalizedRequest, "canonicalization does not work");
            }
            catch (Exception)
            {
                Console.WriteLine("#######################################################################");
                Console.WriteLine(testFixture.expected.canonicalizedRequest);
                Console.WriteLine("#######################################################################");
                Console.WriteLine(canonicalizedRequest);
                Console.WriteLine("#######################################################################");
                throw;
            }
        }

        static object[] TestFixtures()
        {
            var files = Directory.GetFiles("TestFixtures/aws4_testsuite")
                .Union(Directory.GetFiles("TestFixtures/emarsys_testsuite"));

            return new object[] { TestFixtureReader.Read(@"TestFixtures/aws4_testsuite/signrequest-post-vanilla-query-space.json") };

            return files.Select(file => (object) TestFixtureReader.Read(file)).ToArray();
        }   
    }
}
