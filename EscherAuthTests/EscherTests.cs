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
            public void SignRequestTest(SigningTestFixture testCase)
            {
                var escher = new Escher { Config = testCase.config.ToEscherConfig() };

                var signedRequest = escher.SignRequest(
                    testCase.request,
                    testCase.config.accessKeyId,
                    testCase.config.apiSecret,
                    testCase.headersToSign,
                    testCase.config.DateTime
                );

                Assert.AreEqual(testCase.expected.request, signedRequest);
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

            [Test(), TestCaseSource("ValidTestFixtures")]
            public void AuthenticateTestForValidRequests(AuthenticationTestFixture testCase)
            {
                var escher = new Escher { Config = testCase.config.ToEscherConfig() };

                var apiKey = escher.Authenticate(testCase.request, testCase.KeyDb, testCase.config.DateTime);
                
                Assert.AreEqual(testCase.expected.apiKey, apiKey);
            }

            static object[] ValidTestFixtures()
            {
                var files = Directory.GetFiles("TestFixtures/aws4_testsuite")
                    .Union(Directory.GetFiles("TestFixtures/emarsys_testsuite"));

                return files
                    .Where(file => file.Contains("authenticate"))
                    .Where(file => file.Contains("-valid-"))
                    .Where(file => !IsOnBlackList(file))
                    .Select(file => (object)TestFixtureReader.ReadAuthFixture(file))
                    .ToArray();
            }

            [Test(), TestCaseSource("InvalidTestFixtures")]
            public void AuthenticateTestForInvalidRequests(AuthenticationTestFixture testCase)
            {
                var escher = new Escher { Config = testCase.config.ToEscherConfig() };

                try
                {
                    escher.Authenticate(testCase.request, testCase.KeyDb, testCase.config.DateTime);

                    Assert.Fail("Authentication should have failed");
                }
                catch (EscherAuthenticationException e)
                {
                    Assert.AreEqual(testCase.expected.error, e.Message);
                }
            }

            static object[] InvalidTestFixtures()
            {
                var files = Directory.GetFiles("TestFixtures/aws4_testsuite")
                    .Union(Directory.GetFiles("TestFixtures/emarsys_testsuite"));

                return files
                    .Where(file => file.Contains("authenticate"))
                    .Where(file => file.Contains("-error-"))
                    .Where(file => !IsOnBlackList(file))
                    .Select(file => (object)TestFixtureReader.ReadAuthFixture(file))
                    .ToArray();
            }
        }

        static bool IsOnBlackList(string file)
        {
            var blackList = new[]
            {
                "authenticate-valid-credential-with-spaces.json", //TODO to be done. or not.
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
