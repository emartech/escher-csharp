using NUnit.Framework;
using EscherAuth;
using System;
using System.Linq;
using EscherAuthTests.Helpers;
using System.IO;

namespace EscherAuthTests
{
    [TestFixture()]
    public class RequestCanonicalizerTests
    {
        [Test(), TestCaseSource("TestFixtures")]
        public void CanonicalizeTest(SigningTestFixture testCase)
        {
            var canonicalizer = new RequestCanonicalizer();
            var canonicalizedRequest = canonicalizer.Canonicalize(testCase.request, testCase.headersToSign, testCase.config.ToEscherConfig());

            Assert.AreEqual(testCase.expected.canonicalizedRequest, canonicalizedRequest, "canonicalization does not work");
        }

        static object[] TestFixtures()
        {
            var files = Directory.GetFiles("TestFixtures/aws4_testsuite")
                .Union(Directory.GetFiles("TestFixtures/emarsys_testsuite"));

            return files
                .Where(file => file.Contains("signrequest"))
                .Where(file => !IsOnBlackList(file))
                .Select(file => (object)TestFixtureReader.ReadSigningFixture(file))
                .ToArray();
        }

        static bool IsOnBlackList(string file)
        {
            var blackList = new[]
            {
                "signrequest-get-slash", // can be done...
                "signrequest-get-vanilla-query-unreserved",
                "signrequest-post-vanilla-query-nonunreserved",
                "signrequest-date-header-should-be-signed-headers",
                "signrequest-support-custom-config" // 2x // TODO what are these?
            };

            return blackList.Any(file.Contains);
        }
    }
}
