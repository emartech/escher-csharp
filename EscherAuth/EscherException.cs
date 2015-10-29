using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscherAuth
{
    public class EscherException : Exception
    {
        public EscherException(string message) : base(message)
        {
        }
    }
}
