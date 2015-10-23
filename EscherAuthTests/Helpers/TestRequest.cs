using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EscherAuth;
using EscherAuth.Request;

namespace EscherAuthTests.Helpers
{
    class TestRequest : IEscherRequest
    {
        public string method { get; set; }
        public string url { get; set; }
        public string[][] headers { get; set; }
        public string body { get; set; }

        public string Method
        {
            get { return method; }
            set { method = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public Header[] Headers
        {
            get { return headers.Select(h => new Header(h[0], h[1])).ToArray(); }
            set { headers = value.Select(h => new string[] { h.Name, h.Value }).ToArray(); }
        }

        public string Body
        {
            get { return body; }
            set { body = value; }
        }
    }
}
