using System;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BoTech.HttpClientHelper
{
    public class HttpRequestHelper
    {
        /// <summary>
        /// Gets or sets the collection of HTTP request headers associated with the request.
        /// </summary>
        public HttpRequestHeaders? Headers { get; set; } = null;

        private string _baseUrl;
        public HttpRequestHelper(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        // ----------------------------------------GET----------------------------------------
        
        /// <summary>
        /// Performs a GET request to the given endpoint (baseUrl + url)
        /// This Method also saves the file to a specific location defined in fileName
        /// </summary>
        /// <param name="fileName">The full path of a file to overwrite or create.</param>
        /// <param name="url">The endpoint url</param>
        /// <returns>The request result.</returns>
        public async Task<RequestResult<dynamic>> HttpGetFile(string url, string fileName)
        {
            using (HttpClient client = BuildHttpClient())
            {
                HttpResponseMessage? response = null;
                try
                {
                    response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }

                    Console.WriteLine($"File downloaded to: {fileName}");
                    return new RequestResult<dynamic>(true, response, fileName, null);
                }
                catch (Exception e)
                {
                    return new RequestResult<dynamic>(false, response, null, e);
                }
            }
        }
        /// <summary>
        /// Performs a GET request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <returns>The request result with parsed json data object</returns>
        public async Task<RequestResult<T>> HttpGetJsonObject<T>(string url)
        {
            RequestResult<dynamic> response = await HttpGet(url);
            if (response.IsSuccess())
            {
                string jsonData = await response.ResponseMessage!.Content.ReadAsStringAsync();
                return new RequestResult<T>(true, response.ResponseMessage, JsonConvert.DeserializeObject<T>(jsonData), null);
            }
            return new RequestResult<T>(false, response.ResponseMessage, default(T), response.Error);
        }
    
        /// <summary>
        /// Sends a request to _baseUrl + url and returns the string returned by that method.
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <returns>The returned string from the api and the request result.</returns>
        public async Task<RequestResult<string>> HttpGetString(string url)
        {
            RequestResult<dynamic> response = await HttpGet(url);
            if (response.IsSuccess())
            {
                string data = await response.ResponseMessage!.Content.ReadAsStringAsync();
                return new RequestResult<string>(true, response.ResponseMessage, data, null);
            }
            return new RequestResult<string>(false, response.ResponseMessage, string.Empty, response.Error);
        }
        /// <summary>
        /// Performs a GET request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <returns>The request result with no parsed json data</returns>
        public async Task<RequestResult<dynamic>> HttpGet(string url)
        {
            return await SendHttpRequest(HttpMethod.Get, url, null);
        }
        
        // ----------------------------------------DELETE----------------------------------------
        
        /// <summary>
        /// Performs a DELETE request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The http content to send</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult<dynamic>> HttpDelete(string url, HttpContent? content)
        {
            return await SendHttpRequest(HttpMethod.Delete, url, content);
        }
        /// <summary>
        /// Performs a DELETE request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult<dynamic>> HttpDeleteJson(string url, object? content)
        {
            return await HttpDelete(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a DELETE request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult<T>> HttpDeleteJsonAndGetJson<T>(string url, object? content)
        {
            return await HttpDeleteContentAndGetJson<T>(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a DELETE request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult<T>> HttpDeleteContentAndGetJson<T>(string url, HttpContent? content)
        {
            return await GetJsonFromRequestResult<T, dynamic>(await HttpDelete(url, content));
        }
        
        // ----------------------------------------PUT----------------------------------------
        
        /// <summary>
        /// Performs a PUT request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The http content</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult<dynamic>> HttpPut(string url, HttpContent? content)
        {
            return await SendHttpRequest(HttpMethod.Put, url, content);
        }
        /// <summary>
        /// Performs a PUT request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult<dynamic>> HttpPutJson(string url, object? content)
        {
            return await HttpPut(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a PUT request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult<T>> HttpPutJsonAndGetJson<T>(string url, object? content)
        {
            return await HttpPutContentAndGetJson<T>(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a PUT request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult<T>> HttpPutContentAndGetJson<T>(string url, HttpContent? content)
        {
            return await GetJsonFromRequestResult<T, dynamic>(await HttpPut(url, content));
        }
        
        // ----------------------------------------PATCH----------------------------------------
        
        /// <summary>
        /// Performs a PATCH request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The http content</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult<dynamic>> HttpPatch(string url, HttpContent? content)
        {
            return await SendHttpRequest(HttpMethod.Patch, url, content);
        }
        /// <summary>
        /// Performs a PATCH request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult<dynamic>> HttpPatchJson(string url, object? content)
        {
            return await HttpPatch(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a PATCH request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult<T>> HttpPatchJsonAndGetJson<T>(string url, object? content)
        {
            return await HttpPatchContentAndGetJson<T>(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a PATCH request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult<T>> HttpPatchContentAndGetJson<T>(string url, HttpContent content)
        {
            return await GetJsonFromRequestResult<T, dynamic>(await HttpPatch(url, content));
        }
        
        // ----------------------------------------POST----------------------------------------
        
        /// <summary>
        /// Performs a POST request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The http content</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult<dynamic>> HttpPost(string url, HttpContent content)
        {
            return await SendHttpRequest(HttpMethod.Post, url, content);
        }
        /// <summary>
        /// Performs a POST request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult<dynamic>> HttpPostJson(string url, object? content)
        {
            return await HttpPost(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a POST request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult<T>> HttpPostJsonAndGetJson<T>(string url, object? content)
        {
            return await HttpPostContentAndGetJson<T>(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a POST request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult<T>> HttpPostContentAndGetJson<T>(string url, HttpContent content)
        {
            return await GetJsonFromRequestResult<T, dynamic>(await HttpPost(url, content));
        }

        private async Task<RequestResult<T>> GetJsonFromRequestResult<T, U>(RequestResult<U> result)
        {
            try
            {
                if (result.IsSuccess() &&  result.ResponseMessage != null)
                    return new RequestResult<T>(true, result.ResponseMessage, await GetJsonObjectFromHttpResponseMessage<T>(result.ResponseMessage), null);
                return new RequestResult<T>(false, result.ResponseMessage, default(T), result.Error);
            }
            catch (Exception e)
            {
                return new RequestResult<T>(false, null, default(T), e);
            }
        }

        private async Task<RequestResult<dynamic>> SendHttpRequest(HttpMethod method, string url, HttpContent? content)
        {
            using (HttpClient client = BuildHttpClient())
            {
                HttpResponseMessage? response = null;
                try
                {
                    Console.Write($"─> 🔄️ Performing {method.Method} request: {_baseUrl + url}");
                    response = await client.SendAsync(new HttpRequestMessage(method, url){Content = content});
                    

                    // Ensure the response is successful
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($"└─> ✅ {method.Method} Response Status: {response.StatusCode} ");

                    return new RequestResult<dynamic>(true, response, null, null);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"└─> ❌ Patch Request error: {e.Message}");
                    return new RequestResult<dynamic>(false, response, null, e);
                }
            }
        }
        private StringContent GetJsonHttpContentFromObject(object? objectToSerialize) => new StringContent(JsonConvert.SerializeObject(objectToSerialize), Encoding.UTF8, "application/json");
        
        private async Task<T?> GetJsonObjectFromHttpResponseMessage<T>(HttpResponseMessage response)
        {
            string jsonData = await response.Content.ReadAsStringAsync();
            if (jsonData.Length > 0)
                return JsonConvert.DeserializeObject<T>(jsonData);
            return default(T);
        }
        private HttpClient BuildHttpClient()
        {
            HttpClientBuilder builder; 
            if (Headers != null)
                builder = new HttpClientBuilder(_baseUrl, Headers);
            else
                builder = new HttpClientBuilder(_baseUrl);
            return builder.Build();
        }
    }
}
