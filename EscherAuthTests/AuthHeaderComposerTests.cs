using NUnit.Framework;
using EscherAuth;
using System.IO;
using System.Linq;
using EscherAuthTests.Helpers;

namespace EscherAuthTests
{
    [TestFixture()]
    public class AuthHeaderComposerTests
    {
        [Test(), TestCaseSource("TestFixtures")]
        public void ComposeTest(SigningTestFixture signingTestFixture)
        {
            var authHeaderComposer = new AuthHeaderComposer();
            var authHeader = authHeaderComposer.Compose(
                signingTestFixture.config.ToEscherConfig(),
                signingTestFixture.config.accessKeyId, 
                signingTestFixture.config.DateTime,
                signingTestFixture.headersToSign,
                signingTestFixture.expected.stringToSign,
                signingTestFixture.config.apiSecret
            );

            Assert.AreEqual(signingTestFixture.expected.authHeader, authHeader);
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
