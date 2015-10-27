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

        class SignRequestTests
        {
            [Test(), TestCaseSource("TestFixtures")]
            public void SignRequestTest(SigningTestFixture signingTestFixture)
            {
                var escher = new Escher
                {
                    Config = signingTestFixture.config.ToEscherConfig()
                };

                var signedRequest = escher.SignRequest(
                    signingTestFixture.request,
                    signingTestFixture.config.accessKeyId,
                    signingTestFixture.config.apiSecret,
                    signingTestFixture.headersToSign,
                    signingTestFixture.config.DateTime
                );

                Assert.AreEqual(signingTestFixture.expected.request, signedRequest);
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
        }

        class AuthenticateTests
        {

            [Test(), TestCaseSource("TestFixtures")]
            public void AuthenticateTest(AuthenticationTestFixture testCase)
            {
                var escher = new Escher
                {
                    Config = testCase.config.ToEscherConfig()
                };

                var apiKey = escher.Authenticate(testCase.request, testCase.KeyDb);
                
                Assert.AreEqual(testCase.expected.apiKey, apiKey);
            }

            static object[] TestFixtures()
            {
                //return new object[] { TestFixtureReader.ReadAuthFixture("TestFixtures/emarsys_testsuite/authenticate-error-invalid-escher-key.json") };

                var files = Directory.GetFiles("TestFixtures/aws4_testsuite")
                    .Union(Directory.GetFiles("TestFixtures/emarsys_testsuite"));

                return files
                    .Where(file => file.Contains("authenticate"))
                    .Where(file => file.Contains("-valid-"))
                    .Where(file => !IsOnBlackList(file))
                    .Select(file => (object)TestFixtureReader.ReadAuthFixture(file))
                    .ToArray();
            }
        }

        static bool IsOnBlackList(string file)
        {
            var blackList = new[]
            {
                "presigned", // not today
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
