using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscherAuth
{
    public class EscherAuthenticationException : EscherException
    {
        public EscherAuthenticationException(string message) : base(message)
        {
        }
    }
}
