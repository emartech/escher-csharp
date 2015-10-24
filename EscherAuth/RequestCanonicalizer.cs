using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using EscherAuth.Request;
using System.Web;
using EscherAuth.Hash;

namespace EscherAuth
{
    public class RequestCanonicalizer
    {
        public string Canonicalize(IEscherRequest request, string[] headersToSign)
        {
            headersToSign = headersToSign.Select(h => h.ToLower()).ToArray();
            

            var uri = new Uri("http://localhost" + request.Url);

            return String.Join("\n", new string[]
            {
                request.Method.ToUpper(),
                uri.AbsolutePath.Replace("//", "/"),
                CanonicalizeQueryString(uri),
            }.Concat(CanonicalizeHeaders(request, headersToSign)).Concat(new string[]
            {
                null,
                String.Join(";", headersToSign.OrderBy(s => s)),
                Hasher.Hash(request.Body)
            }));
        }

        private static IEnumerable<string> CanonicalizeHeaders(IEscherRequest request, string[] headersToSign)
        {
            var headers = request.Headers
                .Select(NormalizeHeader)
                .Where(h => headersToSign.Contains(h.Name))
                .OrderBy(h => h.Name)
                .ToArray();

            var headersAsString = new List<string>();

            foreach (var header in headers)
            {
                if (headersAsString.Any(h => h.Contains(header.Name + ":")))
                    continue;

                var headersWithSameName = headers.Where(h => h.Name == header.Name);

                headersAsString.Add(header.Name + ":" + String.Join(",", headersWithSameName.Select(h => h.Value)));
            }

            return headersAsString;
        }

        private static Header NormalizeHeader(Header h)
        {
            var normalizedName = h.Name.ToLower();
            var normalizedValue = h.Value.FirstOrDefault() == '"' && h.Value.LastOrDefault() == '"' ?
                h.Value : 
                Regex.Replace(h.Value, "([ ]{2,})", " ");

            return new Header(normalizedName, normalizedValue);
        }

        private static string CanonicalizeQueryString(Uri uri)
        {
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            var enumerableQueryParams = new List<Tuple<string, string[]>>();
            for (var i = 0; i < queryParams.Count; i++)
            {
                var key = UrlEncodeUpperCase(queryParams.Keys[i]);
                var values = (queryParams.GetValues(i) ?? new string[] { }).Select(UrlEncodeUpperCase).ToArray();

                Array.Sort(values, StringComparer.Ordinal);
                enumerableQueryParams.Add(key == null
                    ? new Tuple<string, string[]>(values.FirstOrDefault(), new string[] {""})
                    : new Tuple<string, string[]>(key, values));
            }
            return String.Join("&", enumerableQueryParams.Select(param => String.Join("&", param.Item2.Select(value => param.Item1 + "=" + value))));
        }

        private static string UrlEncodeUpperCase(string value)
        {
            if (value == null)
            {
                return null;
            }

            value = HttpUtility.UrlEncode(value);
            return Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
        }
    }
}
