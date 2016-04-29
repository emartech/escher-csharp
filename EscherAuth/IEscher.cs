using System;
using EscherAuth.Request;

namespace EscherAuth
{
    public interface IEscher
    {
        EscherConfig Config { get; set; }
        TRequest SignRequest<TRequest>(TRequest request, string key, string secret, string[] headersToSign = null) where TRequest : IEscherRequest;
        TRequest SignRequest<TRequest>(TRequest request, string key, string secret, string[] headersToSign, DateTime currentTime) where TRequest : IEscherRequest;
        string Authenticate(IEscherRequest request, IKeyDb keyDb);
        string Authenticate(IEscherRequest request, IKeyDb keyDb, DateTime currentTime);
    }
}
