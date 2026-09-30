using System;
using System.Net.Http;

namespace BoTech.HttpClientHelper
{
    public class RequestResult
    {
        /// <summary>
        /// The response Message from the http request
        /// </summary>
        public HttpResponseMessage? ResponseMessage { get; set; }
        /// <summary>
        /// Data object which has been parsed from the http request
        /// </summary>
        public object? ParsedData { get; set; }
        /// <summary>
        /// The Error of the request.
        /// </summary>
        public Exception? Error { get; set; }
        /// <summary>
        /// When the request was successful this var will be true.
        /// </summary>
        private bool _success = false;
        private RequestResult(bool success, HttpResponseMessage? message, object? data, Exception? exception)
        {
            _success = success;
            ResponseMessage = message;
            ParsedData = data;
            Error = exception;
        }

        public static RequestResult SuccessFactory(HttpResponseMessage? message)
        {
            return SuccessFactory(message, null);
        }

        public static RequestResult SuccessFactory(HttpResponseMessage? message, object? data)
        {
            return new RequestResult(true, message, data, null);
        }

        public static RequestResult ErrorFactory(HttpResponseMessage? message, Exception? exception)
        {
            return new RequestResult(false, message, null, exception);
        }
        
        public static RequestResult ErrorFactory(HttpResponseMessage? message, Exception? exception, object? data)
        {
            return new RequestResult(false, message, data, exception);
        }
        
        public bool IsSuccess()
        {
            return _success && Error == null && ResponseMessage != null && ResponseMessage.IsSuccessStatusCode;
        }
    }
}
