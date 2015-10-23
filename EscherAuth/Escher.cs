using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EscherAuth.Request;

namespace EscherAuth
{
    public class Escher
    {
        public string Hello()
        {
            return "Hello World";
        }

        public const string UnsignedPayload = "UNSIGNED-PAYLOAD";
        public const int DefaultPresignExpiration = 86400;

        public string CredentialScope { get; set; }
        public string AlgoPrefix { get; set; } = "ESR";
        public string VendorKey { get; set; } = "Escher";
        public string HashAlgo { get; set; } = "SHA256";
        public DateTime CurrentTime { get; set; } = DateTime.Now;
        public string AuthHeaderName { get; set; } = "X-Escher-Auth";
        public string DateHeaderName { get; set; } = "X-Escher-Date";
        public int ClockSkew { get; set; } = 900;

        public IEscherRequest SignRequest(IEscherRequest request, String key, String secret, List<String> signedHeaders)
        {
            return request;
        }
    }
}
