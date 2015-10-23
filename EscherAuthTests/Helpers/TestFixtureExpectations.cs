using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscherAuthTests.Helpers
{
    class TestFixtureExpectations
    {
        public TestRequest request { get; set; }
        public string canonicalizedRequest { get; set; }
        public string stringToSign { get; set; }
        public string authHeader { get; set; }
    }
}
