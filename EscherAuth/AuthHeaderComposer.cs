using System;
using System.Linq;

namespace EscherAuth
{
    public class AuthHeaderComposer
    {
        public string Compose(EscherConfig config, string key, DateTime dateTime, string[] headersToSign, string stringToSign, string secret)
        {
            Array.Sort(headersToSign, StringComparer.Ordinal);

            return String.Join(" ", new[]
            {
                config.AlgorithmPrefix + "-HMAC-" + config.HashAlgorithm.ToUpper(),
                string.Format("Credential={0}/{1}/{2},", key, dateTime.ToEscherShortDate(), config.CredentialScope),
                string.Format("SignedHeaders={0},", String.Join(";", headersToSign.Select(h => h.ToLower()))),
                "Signature=" + SignatureCalculator.Sign(stringToSign, secret, dateTime, config)
            }); 
        }

    }
}
