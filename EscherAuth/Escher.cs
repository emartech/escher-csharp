using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using EscherAuth.Request;

namespace EscherAuth
{
    public class Escher
    {
        private const string UnsignedPayload = "UNSIGNED-PAYLOAD";
        private const int DefaultPresignExpiration = 86400;

        public EscherConfig Config { get; set; } = new EscherConfig();

        public IEscherRequest SignRequest(IEscherRequest request, string key, string secret, string[] headersToSign = null)
        {
            return SignRequest(request, key, secret, headersToSign ?? new string[] { }, DateTime.Now);
        }

        public IEscherRequest SignRequest(IEscherRequest request, string key, string secret, string[] headersToSign, DateTime currentTime)
        {
            currentTime = currentTime.ToUniversalTime();

            AddMandatoryHeaders(request, currentTime);
            headersToSign = AddMandatoryHeadersToSign(headersToSign);

            var canonicalizedRequest = new RequestCanonicalizer().Canonicalize(request, headersToSign);
            var stringToSign = new StringToSignComposer().Compose(canonicalizedRequest, currentTime, Config);
            var authHeader = new AuthHeaderComposer().Compose(Config, key, currentTime, headersToSign, stringToSign, secret);

            request.Headers.Add(new Header(Config.AuthHeaderName, authHeader));

            return request;
        }

        private void AddMandatoryHeaders(IEscherRequest request, DateTime currentTime)
        {
            var dateHeaderPresent = request.Headers.Any(header => String.Equals(header.Name, Config.DateHeaderName, StringComparison.CurrentCultureIgnoreCase));
            if (!dateHeaderPresent)
            {
                request.Headers.Add(new Header(Config.DateHeaderName, currentTime.ToEscherLongDate()));
            }

            var hostHeaderPresent = request.Headers.Any(header => String.Equals(header.Name, "host", StringComparison.CurrentCultureIgnoreCase));
            if (!hostHeaderPresent)
            {
                request.Headers.Add(new Header("host", request.Uri.Host + (request.Uri.IsDefaultPort ? "" : ":" + request.Uri.Port)));
            }
        }

        private string[] AddMandatoryHeadersToSign(string[] givenHeadersToSign)
        {
            IEnumerable<string> headersToSign = givenHeadersToSign;

            if (!givenHeadersToSign.Any(headerName => String.Equals(Config.DateHeaderName, headerName, StringComparison.CurrentCultureIgnoreCase)))
            {
                headersToSign = headersToSign.Concat(new[] { Config.DateHeaderName });
            }

            if (!givenHeadersToSign.Any(headerName => String.Equals("host", headerName, StringComparison.CurrentCultureIgnoreCase)))
            {
                headersToSign = headersToSign.Concat(new[] { "host" });
            }

            return headersToSign.ToArray();
        }

        public string Authenticate(IEscherRequest request, IKeyDb keyDb)
        {
            var authHeader = request.Headers.FirstOrDefault(header => header.Name == Config.AuthHeaderName);
            if (authHeader == null)
            {
                return "auth header missing";
            }

            var authHeaderParts = authHeader.Value.Split(' ');
            var algorythms = authHeaderParts[0];
            var credentialParts = authHeaderParts[1].Replace("Credential=", "").Split('/');
            var apiKey = credentialParts[0];


            return apiKey;
        }

    }
}
