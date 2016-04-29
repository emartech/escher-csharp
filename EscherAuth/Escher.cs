using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EscherAuth.Request;

namespace EscherAuth
{
    public class Escher : IEscher
    {
        private const string UnsignedPayload = "UNSIGNED-PAYLOAD";
        private const int DefaultPresignExpiration = 86400;

        public EscherConfig Config { get; set; } = new EscherConfig();

        public TRequest SignRequest<TRequest>(TRequest request, string key, string secret, string[] headersToSign = null) where TRequest : IEscherRequest
        {
            return SignRequest(request, key, secret, headersToSign ?? new string[] { }, DateTime.Now);
        }

        public TRequest SignRequest<TRequest>(TRequest request, string key, string secret, string[] headersToSign, DateTime currentTime) where TRequest : IEscherRequest
        {
            currentTime = currentTime.ToUniversalTime();

            AddMandatoryHeaders(request, currentTime);
            headersToSign = AddMandatoryHeadersToSign(headersToSign);

            var canonicalizedRequest = new RequestCanonicalizer().Canonicalize(request, headersToSign, Config);
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
            return Authenticate(request, keyDb, DateTime.Now);
        }

        public string Authenticate(IEscherRequest request, IKeyDb keyDb, DateTime currentTime)
        {
            currentTime = currentTime.ToUniversalTime();

            var authHeader = request.Headers.FirstOrDefault(header => header.Name == Config.AuthHeaderName);
            if (authHeader == null)
            {
                throw new EscherAuthenticationException("The authorization header is missing");
            }

            var dateHeader = request.Headers.FirstOrDefault(header => header.Name == Config.DateHeaderName);
            if (dateHeader == null)
            {
                throw new EscherAuthenticationException("The date header is missing");
            }

            var hostHeader = request.Headers.FirstOrDefault(header => header.Name.ToLower() == "host");
            if (hostHeader == null)
            {
                throw new EscherAuthenticationException("The host header is missing");
            }

            var match = Regex.Match(authHeader.Value, "^([^\\-]+)-HMAC-(SHA[\\d]+) Credential=([^/]+)/([\\d]{8})/([^,]+), SignedHeaders=([^,]+), Signature=([a-z0-9]+)$");

            if (!match.Success)
            {
                throw new EscherAuthenticationException("Could not parse auth header");
            }

            var algorythmPrefix = match.Groups[1].Value;
            var hashAlgorythm = match.Groups[2].Value;
            var apiKey = match.Groups[3].Value;
            var shortDate = match.Groups[4].Value;
            var credentialScope = match.Groups[5].Value;
            var signedHeaders = match.Groups[6].Value.Split(';');
            var signature = match.Groups[7].Value;

            DateTime requestTime;
            try
            {
                requestTime = DateTime.Parse(dateHeader.Value).ToUniversalTime();
            }
            catch (FormatException)
            {
                requestTime = DateTimeParser.FromEscherLongDate(dateHeader.Value);
            }
            if (requestTime.ToUniversalTime().Date != DateTimeParser.FromEscherShortDate(shortDate).Date)
            {
                throw new EscherAuthenticationException("The Authorization header's shortDate does not match with the request date");
            }
            if (Math.Abs((currentTime - requestTime).TotalSeconds) > Config.ClockSkew)
            {
                throw new EscherAuthenticationException("The request date is not within the accepted time range");
            }

            if (credentialScope != Config.CredentialScope)
            {
                throw new EscherAuthenticationException("The credential scope is invalid");
            }

            if (!signedHeaders.Contains(Config.DateHeaderName.ToLower()))
            {
                throw new EscherAuthenticationException("The date header is not signed");
            }

            if (!signedHeaders.Contains("host"))
            {
                throw new EscherAuthenticationException("The host header is not signed");
            }
                
            if (!new[] { "SHA256", "SHA512" }.Contains(hashAlgorythm.ToUpper()))
            {
                throw new EscherAuthenticationException("Only SHA256 and SHA512 hash algorithms are allowed");
            }

            var secret = keyDb[apiKey];
            if (secret == null)
            {
                throw new EscherAuthenticationException("Invalid Escher key");
            }

            var canonicalizedRequest = new RequestCanonicalizer().Canonicalize(request, signedHeaders, Config);
            var stringToSign = new StringToSignComposer().Compose(canonicalizedRequest, requestTime, Config);
            var calculatedSignature = SignatureCalculator.Sign(stringToSign, secret, requestTime, Config);
            if (calculatedSignature != signature)
            {
                throw new EscherAuthenticationException("The signatures do not match");
            }

            return apiKey;
        }

    }
}
