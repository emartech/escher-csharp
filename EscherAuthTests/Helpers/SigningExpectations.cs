namespace EscherAuthTests.Helpers
{
    public class SigningExpectations
    {
        public TestRequest request { get; set; }
        public string canonicalizedRequest { get; set; }
        public string stringToSign { get; set; }
        public string authHeader { get; set; }
    }
}
