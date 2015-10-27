using EscherAuth;

namespace EscherAuthTests.Helpers
{
    public class AuthenticationTestFixture
    {
        public string title { get; set; }
        public string[] headersToSign { get; set; }
        public TestRequest request { get; set; }
        public TestEscherConfig config { get; set; }
        public AuthenticationExpectations expected { get; set; }
        public string[][] keyDb { get; set; }

        public IKeyDb KeyDb => new TestKeyDb(keyDb);

        public override string ToString()
        {
            return title;
        }
    }
}
