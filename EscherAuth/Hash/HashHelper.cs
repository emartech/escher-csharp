using System;
using System.Security.Cryptography;
using System.Text;

namespace EscherAuth.Hash
{
    class HashHelper
    {
        public static string Hash(string subject, string hashAlgorithm = "TODO: to be implemented to be required")
        {
            return ByteArrayToHexaString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(subject)));
        }

        public static HMAC GetHMacImplementation(string hashAlgorithm)
        {
            return HMAC.Create("HMAC" + hashAlgorithm.ToUpper());
        }

        public static string ByteArrayToHexaString(byte[] hashAsByteArray)
        {
            return BitConverter
                .ToString(hashAsByteArray)
                .Replace("-", "")
                .ToLower();
        }
    }
}
