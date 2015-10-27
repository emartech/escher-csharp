namespace EscherAuthTests.Helpers
{
    public class SigningTestFixture
    {
        public string title { get; set; }
        public string[] headersToSign { get; set; }
        public TestRequest request { get; set; }
        public TestEscherConfig config { get; set; }
        public SigningExpectations expected { get; set; }

        public override string ToString()
        {
            return title;
        }
    }
}
