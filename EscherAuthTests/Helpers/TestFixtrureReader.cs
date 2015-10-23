using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace EscherAuthTests.Helpers
{
    class TestFixtureReader
    {
        public static TestFixture Read(string fileName)
        {
            var testFixture = JsonConvert.DeserializeObject<TestFixture>(File.ReadAllText(fileName));
            testFixture.title = fileName;
            return testFixture;
        }
    }
}
