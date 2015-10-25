using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            return String.Join("\n", new[]
            {
                request.Method.ToUpper(),
                request.Uri.AbsolutePath,
                CanonicalizeQueryString(request.Uri),
            }.Concat(CanonicalizeHeaders(request, headersToSign)).Concat(new[]
            {
                null,
                String.Join(";", headersToSign.OrderBy(s => s)),
                HashHelper.Hash(request.Body)
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
                var currentHeader = header;

                if (headersAsString.Any(h => h.Contains(currentHeader.Name + ":")))
                    continue;

                var headersWithSameName = headers.Where(h => h.Name == currentHeader.Name);

                headersAsString.Add(currentHeader.Name + ":" + String.Join(",", headersWithSameName.Select(h => h.Value)));
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
                    ? new Tuple<string, string[]>(values.FirstOrDefault(), new[] {""})
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
