using System;
using System.Net.Http;

namespace BoTech.HttpClientHelper
{
    public class RequestResult<T>(bool success, HttpResponseMessage? message, T? data, Exception? exception)
    {
        /// <summary>
        /// The response Message from the http request
        /// </summary>
        public HttpResponseMessage? ResponseMessage { get; set; } = message;
        /// <summary>
        /// Data object which has been parsed from the http request
        /// </summary>
        public T? ParsedData { get; set; } = data;
        /// <summary>
        /// When the request was successful this var will be true.
        /// </summary>
        public bool Success { get; set; } = success;
        /// <summary>
        /// The Error of the request.
        /// </summary>
        public Exception? Error { get; set; } = exception;

        public bool IsSuccess()
        {
            return Success && Error == null && ResponseMessage != null && ResponseMessage.IsSuccessStatusCode;
        }
    }
}
