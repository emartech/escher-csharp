using NUnit.Framework;
using EscherAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EscherAuthTests.Helpers;

namespace EscherAuthTests
{
    [TestFixture()]
    public class EscherTests
    {
        [Test(), TestCaseSource("TestFixtures")]
        public void SignRequestTest(TestFixture testFixture)
        {
            var escher = new Escher
            {
                Config = testFixture.config.ToEscherConfig()
            };

            var signedRequest = escher.SignRequest(
                testFixture.request,
                testFixture.config.accessKeyId,
                testFixture.config.apiSecret,
                testFixture.headersToSign,
                testFixture.config.DateTime
            );

            Assert.AreEqual(testFixture.expected.request, signedRequest);
        }

        static object[] TestFixtures()
        {
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
                "signrequest-get-slash", // can be done...
                "signrequest-get-vanilla-query-unreserved",
                "signrequest-post-vanilla-query-nonunreserved",
                "signrequest-post-header-value-spaces", // 2x // why is it modifying the original headers?
                "signrequest-date-header-should-be-signed-headers", // Why does it specify that datetime format in the date header?
                "signrequest-support-custom-config" // 2x // TODO what are these?
            };

            return blackList.Any(file.Contains);
        }
    }
}
