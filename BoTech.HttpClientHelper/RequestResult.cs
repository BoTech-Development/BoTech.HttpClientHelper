using System;
using System.Net.Http;

namespace BoTech.HttpClientHelper
{
    public class RequestResult<T>
    {
        /// <summary>
        /// The response Message from the http request
        /// </summary>
        public HttpResponseMessage? ResponseMessage { get; set; }
        /// <summary>
        /// Data object which has been parsed from the http request
        /// </summary>
        public T? ParsedData { get; set; }
        /// <summary>
        /// The Error of the request.
        /// </summary>
        public Exception? Error { get; set; }
        /// <summary>
        /// When the request was successful this var will be true.
        /// </summary>
        private bool _success = false;
        private RequestResult(bool success, HttpResponseMessage? message, T? data, Exception? exception)
        {
            _success = success;
            ResponseMessage = message;
            ParsedData = data;
            Error = exception;
        }

        public static RequestResult<T> SuccessFactory(HttpResponseMessage? message)
        {
            return SuccessFactory( message, default(T));
        }

        public static RequestResult<T> SuccessFactory(HttpResponseMessage? message, T? data)
        {
            return new RequestResult<T>(true, message, data, null);
        }

        public static RequestResult<T> ErrorFactory(HttpResponseMessage? message, Exception? exception)
        {
            return new RequestResult<T>(false, message, default(T), exception);
        }
        
        public bool IsSuccess()
        {
            return _success && Error == null && ResponseMessage != null && ResponseMessage.IsSuccessStatusCode;
        }
    }
}
