using System;
using System.Security.Cryptography;
using System.Text;

namespace EscherAuth.Hash
{
    static class HashHelper
    {
        public static string Hash(string subject, string hashAlgorithm)
        {
            HashAlgorithm hasher;
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA256":
                    hasher = SHA256.Create();
                    break;
                case "SHA512":
                    hasher = SHA512.Create();
                    break;
                default:
                    throw new EscherException("Invalid hash algorythm: " + hashAlgorithm);
            }
            
            return ByteArrayToHexaString(hasher.ComputeHash(Encoding.UTF8.GetBytes(subject)));
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
