namespace EscherAuth
{
    public class EscherConfig
    {
        public string CredentialScope { get; set; } = "dafault/scope";
        public string VendorKey { get; set; } = "Escher";
        public string AlgorithmPrefix { get; set; } = "ESR";
        public string HashAlgorithm { get; set; } = "SHA256";

        public string AuthHeaderName { get; set; } = "X-Escher-Auth";
        public string DateHeaderName { get; set; } = "X-Escher-Date";

        public int ClockSkew { get; set; } = 900;

    }
}
