using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EscherAuth.Hash;

namespace EscherAuth
{
    public class StringToSignComposer
    {
        public string Compose(string canonicalizedRequest, DateTime dateTime, EscherConfig config)
        {
            return String.Join("\n", new string[]
            {
                config.AlgorithmPrefix.ToUpper() + "-HMAC-" + config.HashAlgorithm.ToUpper(),
                dateTime.ToString("yyyyMMddTHHmmssZ"),
                dateTime.ToString("yyyyMMdd") + "/" + config.CredentialScope,
                Hasher.Hash(canonicalizedRequest, config.HashAlgorithm)
            });
        }
    }
}
