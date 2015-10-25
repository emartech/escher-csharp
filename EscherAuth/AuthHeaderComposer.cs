using System;
using System.Linq;
using System.Text;
using EscherAuth.Hash;

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
                "Signature=" + CalculateSignature(stringToSign, secret, dateTime, config)
            }); 
        }

        private static string CalculateSignature(string stringToSign, string secret, DateTime dateTime, EscherConfig config)
        {
            var hmac = HashHelper.GetHMacImplementation(config.HashAlgorithm);

            hmac.Key = Encoding.UTF8.GetBytes(config.AlgorithmPrefix + secret);
            hmac.Key = hmac.ComputeHash(Encoding.UTF8.GetBytes(dateTime.ToEscherShortDate()));

            foreach (var credentialScopePart in config.CredentialScope.Split('/'))
            {
                hmac.Key = hmac.ComputeHash(Encoding.UTF8.GetBytes(credentialScopePart));
            }

            return HashHelper.ByteArrayToHexaString(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
        }

    }
}
