using System;
using System.Text;
using EscherAuth.Hash;

namespace EscherAuth
{
    public class SignatureCalculator
    {
        public static string Sign(string stringToSign, string secret, DateTime dateTime, EscherConfig config)
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
