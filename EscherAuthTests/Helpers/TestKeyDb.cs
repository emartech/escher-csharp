using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EscherAuth;

namespace EscherAuthTests.Helpers
{
    class TestKeyDb : IKeyDb
    {
        private readonly string[][] keyDb;

        public TestKeyDb(string[][] keyDb)
        {
            this.keyDb = keyDb;
        }

        public string this[string key]
        {
            get { return keyDb.FirstOrDefault(keySecretPair => keySecretPair[0] == key)?[1]; }
        }
    }
}
