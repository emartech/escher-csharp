using System;
using EscherAuth;

namespace EscherAuthTests.Helpers
{
    public class TestEscherConfig
    {
        public string vendorKey {get;set;} = "AWS4";
        public string algoPrefix {get;set;} = "AWS4";
        public string hashAlgo {get;set;} = "SHA256";
        public string credentialScope {get;set;} = "us-east-1/host/aws4_request";
        public string apiSecret {get;set;} = "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY";
        public string accessKeyId {get;set;} = "AKIDEXAMPLE";
        public string authHeaderName {get;set;} = "Authorization";
        public string dateHeaderName {get;set;} = "Date";
        public string date { get; set; } = "2011-09-09T23 {get;set;} =36 {get;set;} =00.000Z";

        public DateTime DateTime
        {
            get { return DateTime.Parse(date).ToUniversalTime(); }
        }

        public EscherConfig ToEscherConfig()
        {
            return new EscherConfig
            {
                CredentialScope = credentialScope,
                AlgorithmPrefix = algoPrefix,
                HashAlgorithm = hashAlgo,
                VendorKey = vendorKey
            };
        }
    }
}
