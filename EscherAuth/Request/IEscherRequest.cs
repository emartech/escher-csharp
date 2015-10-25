using System;
using System.Collections.Generic;

namespace EscherAuth.Request
{
    public interface IEscherRequest
    {
        string Method { get; }
        Uri Uri { get; }
        List<Header> Headers { get; }
        string Body { get; }
    }
}
