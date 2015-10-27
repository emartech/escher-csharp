using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscherAuth
{
    public interface IKeyDb
    {
        string this[string key]
        {
            get;
        }
    }
}
