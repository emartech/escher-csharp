using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscherAuth
{
    public class EscherAuthenticationException : Exception
    {
        public EscherAuthenticationException(string message) : base(message)
        {
        }
    }
}
