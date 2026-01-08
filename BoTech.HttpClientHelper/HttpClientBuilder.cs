using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BoTech.HttpClientHelper
{
    /// <summary>
    /// This class is only neccessary because HttpClient does not provide a setter for the DefaultRequestHeaders property.
    /// ( ˘︹˘ )(ㆆ_ㆆ)
    /// </summary>
    public class HttpClientBuilder
    {   
        private string _baseUrl;
        private HttpMessageHandler _httpMessageHandler;
        private HttpRequestHeaders _headers;
        public HttpClientBuilder(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        public HttpClientBuilder(string baseUrl, HttpRequestHeaders headers)
        {
            _baseUrl = baseUrl;
            _headers = headers;
        }
        public HttpClient Build()
        {
            HttpClient client = new HttpClient();
            
            client.BaseAddress = new Uri(_baseUrl);
            if(_headers != null)
            {
                ApplyHeaders(client);
                ApplySpecificHeaders(client);
            }
            return client;
        }
        private void ApplyHeaders(HttpClient client)
        {
       
            client.DefaultRequestHeaders.Accept.Concat(_headers.Accept);
            client.DefaultRequestHeaders.AcceptCharset.Concat(_headers.AcceptCharset);
            client.DefaultRequestHeaders.AcceptEncoding.Concat(_headers.AcceptEncoding);
            client.DefaultRequestHeaders.AcceptLanguage.Concat(_headers.AcceptLanguage);
            client.DefaultRequestHeaders.Connection.Concat(_headers.Connection);
            client.DefaultRequestHeaders.Expect.Concat(_headers.Expect);
            client.DefaultRequestHeaders.IfMatch.Concat(_headers.IfMatch);
            client.DefaultRequestHeaders.IfNoneMatch.Concat(_headers.IfNoneMatch);
            client.DefaultRequestHeaders.Pragma.Concat(_headers.Pragma);
            client.DefaultRequestHeaders.TE.Concat(_headers.TE);
            client.DefaultRequestHeaders.Trailer.Concat(_headers.Trailer);
            client.DefaultRequestHeaders.TransferEncoding.Concat(_headers.TransferEncoding);
            client.DefaultRequestHeaders.Upgrade.Concat(_headers.Upgrade);
            client.DefaultRequestHeaders.UserAgent.Concat(_headers.UserAgent);
            client.DefaultRequestHeaders.Via.Concat(_headers.Via);
            client.DefaultRequestHeaders.Warning.Concat(_headers.Warning);



            client.DefaultRequestHeaders.Authorization = _headers.Authorization;
            client.DefaultRequestHeaders.CacheControl = _headers.CacheControl;
            client.DefaultRequestHeaders.ConnectionClose = _headers.ConnectionClose;
            client.DefaultRequestHeaders.Date = _headers.Date;
            client.DefaultRequestHeaders.ExpectContinue = _headers.ExpectContinue;
            client.DefaultRequestHeaders.From = _headers.From;
            client.DefaultRequestHeaders.Host = _headers.Host;
            client.DefaultRequestHeaders.IfModifiedSince = _headers.IfModifiedSince;
            client.DefaultRequestHeaders.IfRange = _headers.IfRange;
            client.DefaultRequestHeaders.IfUnmodifiedSince = _headers.IfUnmodifiedSince;
            client.DefaultRequestHeaders.MaxForwards = _headers.MaxForwards;
            client.DefaultRequestHeaders.Protocol = _headers.Protocol;
            client.DefaultRequestHeaders.ProxyAuthorization = _headers.ProxyAuthorization;
            client.DefaultRequestHeaders.Range = _headers.Range;
            client.DefaultRequestHeaders.Referrer = _headers.Referrer;
            client.DefaultRequestHeaders.TransferEncodingChunked = _headers.TransferEncodingChunked;

        }

        private void ApplySpecificHeaders(HttpClient client)
        {
            foreach (var header in _headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
    }
}
