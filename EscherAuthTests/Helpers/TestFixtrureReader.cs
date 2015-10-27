using System.IO;
using Newtonsoft.Json;

namespace EscherAuthTests.Helpers
{
    class TestFixtureReader
    {
        public static SigningTestFixture ReadSigningFixture(string fileName)
        {
            var testFixture = JsonConvert.DeserializeObject<SigningTestFixture>(File.ReadAllText(fileName));
            testFixture.title = fileName;
            return testFixture;
        }

        public static AuthenticationTestFixture ReadAuthFixture(string fileName)
        {
            var testFixture = JsonConvert.DeserializeObject<AuthenticationTestFixture>(File.ReadAllText(fileName));
            testFixture.title = fileName;
            return testFixture;
        }
    }
}
