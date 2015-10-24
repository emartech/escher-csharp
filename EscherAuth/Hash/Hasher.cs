using System;
using System.Security.Cryptography;
using System.Text;

namespace EscherAuth.Hash
{
    class Hasher
    {
        public static string Hash(string subject, string hashAlgorithm = "TODO: to be implemented to be required")
        {
            return BitConverter
                    .ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(subject)))
                    .Replace("-", "")
                    .ToLower();
        }
    }
}
