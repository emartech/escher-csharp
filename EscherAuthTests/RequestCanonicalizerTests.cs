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
                Console.WriteLine(testFixture.title);
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
            // return new object[] { TestFixtureReader.Read(@"TestFixtures/emarsys_testsuite\signrequest-get-header-key-duplicate.json") };

            var files = Directory.GetFiles("TestFixtures/aws4_testsuite")
                .Union(Directory.GetFiles("TestFixtures/emarsys_testsuite"));

            return files
                .Where(file => file.Contains("signrequest"))
                .Where(file => !IsOnBlackList(file))
                .Select(file => (object) TestFixtureReader.Read(file))
                .ToArray();
        }

        static bool IsOnBlackList(string file)
        {
            var blackList = new string[]
            {
                "signrequest-get-vanilla-query-unreserved",
                "signrequest-post-vanilla-query-nonunreserved",
                "signrequest-date-header-should-be-signed-headers", // WARNING do not exclude in e2e tests
                "signrequest-support-custom-config" // 2x // TODO what are these?
            };

            return blackList.Any(file.Contains);
        }
    }
}
