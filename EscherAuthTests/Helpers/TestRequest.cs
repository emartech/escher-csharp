using System;
using System.Collections.Generic;
using System.Linq;
using EscherAuth.Request;

namespace EscherAuthTests.Helpers
{
    public class TestRequest : IEscherRequest
    {
        public string method { get; set; }
        public string url { get; set; }
        public string[][] headers { get { return Headers.Select(h => new[] { h.Name, h.Value }).ToArray(); } set { Headers = value.Select(h => new Header(h[0], h[1])).ToList(); } }
        public string body { get; set; }

        public string Method => method;

        public Uri Uri
        {
            get { return new Uri( "http://" + Headers.FirstOrDefault(h => h.Name.ToLower() == "host").Value + url); }
        }

        public List<Header> Headers { get; private set; } = new List<Header>();

        public string Body => body;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var request = obj as TestRequest;
            if (request == null)
            {
                return false;
            }

            return request.method.Equals(method)
                && request.url.Equals(url)
                && request.Headers.Count.Equals(Headers.Count) && request.Headers.All(h => Headers.Contains(h))
                && request.body.Equals(body);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + method.GetHashCode();
            hash = (hash * 7) + url.GetHashCode();
            hash = (hash * 7) + headers.GetHashCode();
            hash = (hash * 7) + body.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}", method, url, "    " + String.Join("\n    ", Headers.Select(h => h.Name + ":" + h.Value)), body);
        }
    }
}
