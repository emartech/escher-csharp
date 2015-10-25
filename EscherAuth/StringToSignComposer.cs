using System;
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
                dateTime.ToEscherLongDate(),
                dateTime.ToEscherShortDate() + "/" + config.CredentialScope,
                HashHelper.Hash(canonicalizedRequest, config.HashAlgorithm)
            });
        }
    }
}
