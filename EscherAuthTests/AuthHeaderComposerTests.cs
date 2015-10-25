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
        public void ComposeTest(TestFixture testFixture)
        {
            var authHeaderComposer = new AuthHeaderComposer();
            var authHeader = authHeaderComposer.Compose(
                testFixture.config.ToEscherConfig(),
                testFixture.config.accessKeyId, 
                testFixture.config.DateTime,
                testFixture.headersToSign,
                testFixture.expected.stringToSign,
                testFixture.config.apiSecret
            );

            Assert.AreEqual(testFixture.expected.authHeader, authHeader);
        }

        static object[] TestFixtures()
        {
            // return new object[] { TestFixtureReader.Read(@"TestFixtures/aws4_testsuite\signrequest-get-vanilla.json") };

            var files = Directory.GetFiles("TestFixtures/aws4_testsuite")
                .Union(Directory.GetFiles("TestFixtures/emarsys_testsuite"));

            return files
                .Where(file => file.Contains("signrequest"))
                .Where(file => !IsOnBlackList(file))
                .Select(file => (object)TestFixtureReader.Read(file))
                .ToArray();
        }

        static bool IsOnBlackList(string file)
        {
            var blackList = new[]
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
