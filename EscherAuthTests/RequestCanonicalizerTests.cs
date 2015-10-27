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
        public void CanonicalizeTest(SigningTestFixture signingTestFixture)
        {
            var canonicalizer = new RequestCanonicalizer();
            var canonicalizedRequest = canonicalizer.Canonicalize(signingTestFixture.request, signingTestFixture.headersToSign);

            try
            {
                Assert.AreEqual(signingTestFixture.expected.canonicalizedRequest, canonicalizedRequest, "canonicalization does not work");
            }
            catch (Exception)
            {
                Console.WriteLine(signingTestFixture.title);
                Console.WriteLine("#######################################################################");
                Console.WriteLine(signingTestFixture.expected.canonicalizedRequest);
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
                "signrequest-date-header-should-be-signed-headers", // WARNING do not exclude in e2e tests
                "signrequest-support-custom-config" // 2x // TODO what are these?
            };

            return blackList.Any(file.Contains);
        }
    }
}
