using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscherAuth
{
    public class EscherConfig
    {
        public string CredentialScope { get; set; }
        public string VendorKey { get; set; }
        public string AlgorithmPrefix { get; set; }
        public string HashAlgorithm { get; set; }

    }
}
