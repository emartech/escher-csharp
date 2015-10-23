using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using EscherAuth.Request;
using System.Web;

namespace EscherAuth
{
    public class RequestCanonicalizer
    {
        public string Canonicalize(IEscherRequest request, string[] headersToSign)
        {
            var hasher = SHA256.Create();

            var uri = new Uri("http://localhost" + request.Url);

            return String.Join("\n", new string[]
            {
                request.Method.ToUpper(),
                uri.AbsolutePath.Replace("//", "/"),
                CanonicalizeQueryString(uri),
            }.Union(request.Headers.Select(h => h.Name.ToLower() + ":" + h.Value)).Union(new string[]
            {
                null,
                String.Join(";", headersToSign.Select(h => h.ToLower())),
                BitConverter.ToString(hasher.ComputeHash( System.Text.Encoding.UTF8.GetBytes(request.Body))).Replace("-", "").ToLower()
            }));

        }

        private static string CanonicalizeQueryString(Uri uri)
        {
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            var enumerableQueryParams = new List<Tuple<string, string[]>>();
            for (int i = 0; i < queryParams.Count; i++)
            {
                var key = queryParams.Keys[i];
                var values = queryParams.GetValues(i);

                enumerableQueryParams.Add(key == null
                    ? new Tuple<string, string[]>(values.FirstOrDefault(), new string[] {""})
                    : new Tuple<string, string[]>(key, values));
            }
            return String.Join("&", enumerableQueryParams.Select(param => String.Join("&", param.Item2.Select(value => param.Item1 + "=" + value))));
        }
    }
}
