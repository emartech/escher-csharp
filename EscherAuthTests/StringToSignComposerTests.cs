using NUnit.Framework;
using EscherAuth;
using System;
using System.IO;
using System.Linq;
using EscherAuthTests.Helpers;

namespace EscherAuthTests
{
    [TestFixture()]
    public class StringToSignComposerTests
    {
        [Test(), TestCaseSource("TestFixtures")]
        public void ComposeTest(SigningTestFixture signingTestFixture)
        {
            var stringToSignComposer = new StringToSignComposer();
            var stringToSign = stringToSignComposer.Compose(
                signingTestFixture.expected.canonicalizedRequest,
                signingTestFixture.config.DateTime,
                signingTestFixture.config.ToEscherConfig()
            );

            Assert.AreEqual(signingTestFixture.expected.stringToSign, stringToSign, "stringToSign does not work");
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
                "signrequest-get-vanilla-query-unreserved",
                "signrequest-post-vanilla-query-nonunreserved",
                "signrequest-date-header-should-be-signed-headers",
                "signrequest-support-custom-config" // 2x // TODO what are these?
            };

            return blackList.Any(file.Contains);
        }
    }
}
