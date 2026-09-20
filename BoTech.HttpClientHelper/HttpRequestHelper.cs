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
        public async Task<RequestResult<dynamic>> HttpGetFileAndCopyTo(string url, string fileName)
        {
            RequestResult<Stream> innerResult = await HttpGetFileStream(url);
            if(!innerResult.IsSuccess())
                return RequestResult<dynamic>.ErrorFactory(innerResult.ResponseMessage, innerResult.Error);
            
            Console.Write($"──> 🔄️  Writing download result to: {fileName}");
            Stream stream = innerResult.ParsedData!;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) 
            {
                stream.CopyTo(fileStream);
            }
            Console.WriteLine($" └─> ✅ Downloaded File written to: {fileName}");
            return RequestResult<dynamic>.SuccessFactory(innerResult.ResponseMessage);
        }
        /// <summary>
        /// Performs a GET request to the given endpoint (baseUrl + url).
        /// This Method downloads the file and returns the contents as a string.
        /// </summary>
        /// <param name="url">The endpoint Url</param>
        /// <returns>The request result including the read contents as string.</returns>
        public async Task<RequestResult<string>> HttpGetFileContents(string url)
        {
            RequestResult<Stream> innerResult = await HttpGetFileStream(url);
            if (innerResult.IsSuccess())
            {
                string fileContents = await new StreamReader(innerResult.ParsedData).ReadToEndAsync();
                return RequestResult<string>.SuccessFactory(innerResult.ResponseMessage, fileContents);
            }
            return RequestResult<string>.ErrorFactory(innerResult.ResponseMessage, innerResult.Error);
        }
        private async Task<RequestResult<Stream>> HttpGetFileStream(string url)
        {
            using (HttpClient client = BuildHttpClient())
            {
                HttpResponseMessage? response = null;
                try
                {
                    Console.WriteLine($"─> 🔄️ Performing File-Get request: {_baseUrl + url}");
                    
                    response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    Stream stream = await response.Content.ReadAsStreamAsync();
                    Console.WriteLine($"└─> ✅ Downloaded with Response Status: {response.StatusCode} ");
                    return RequestResult<Stream>.SuccessFactory(response, stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"└─> ❌ File-Get Request error: {e.Message}");
                    return RequestResult<Stream>.ErrorFactory(response, e);
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
                return RequestResult<T>.SuccessFactory(response.ResponseMessage, JsonConvert.DeserializeObject<T>(jsonData));
            }
            return RequestResult<T>.ErrorFactory(response.ResponseMessage, response.Error);
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
                return RequestResult<string>.SuccessFactory(response.ResponseMessage, data);
            }
            return RequestResult<string>.ErrorFactory(response.ResponseMessage, response.Error);
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
                    return RequestResult<T>.SuccessFactory(result.ResponseMessage, await GetJsonObjectFromHttpResponseMessage<T>(result.ResponseMessage));
                return RequestResult<T>.ErrorFactory(result.ResponseMessage, result.Error);
            }
            catch (Exception e)
            {
                return RequestResult<T>.ErrorFactory(result.ResponseMessage, e);
            }
        }

        private async Task<RequestResult<dynamic>> SendHttpRequest(HttpMethod method, string url, HttpContent? content)
        {
            using (HttpClient client = BuildHttpClient())
            {
                HttpResponseMessage? response = null;
                try
                {
                    Console.WriteLine($"─> 🔄️ Performing {method.Method} request: {_baseUrl + url}");
                    response = await client.SendAsync(new HttpRequestMessage(method, url){Content = content});
                    

                    // Ensure the response is successful
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($"└─> ✅ {method.Method} Response Status: {response.StatusCode} ");

                    return RequestResult<dynamic>.SuccessFactory(response);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"└─> ❌ {method.Method} Request error: {e.Message}");
                    return RequestResult<dynamic>.ErrorFactory(response, e);
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
