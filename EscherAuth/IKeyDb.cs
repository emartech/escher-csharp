using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscherAuth
{
    public interface IKeyDb
    {
        /// <summary>
        /// Returns the sercet for a key.
        /// </summary>
        /// <param name="key">The name of the key.</param>
        /// <returns>The secret belonging to the key, or null, is the key is unknown.</returns>
        string this[string key]
        {
            get;
        }
    }
}
