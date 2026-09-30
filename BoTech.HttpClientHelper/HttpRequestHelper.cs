using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BoTech.HttpClientHelper.Models;
using BoTech.HttpClientHelper.Services;

namespace BoTech.HttpClientHelper
{
    public class HttpRequestHelper(string baseUrl)
    {
        /// <summary>
        /// Gets or sets the collection of HTTP request headers associated with the request.
        /// </summary>
        private HttpRequestHeaders? Headers { get; set; } = null;

        // ----------------------------------------GET----------------------------------------
        
        /// <summary>
        /// Performs a GET request to the given endpoint (baseUrl + url)
        /// This Method also saves the file to a specific location defined in fileName
        /// </summary>
        /// <param name="fileName">The full path of a file to overwrite or create.</param>
        /// <param name="url">The endpoint url</param>
        /// <returns>The request result.</returns>
        public async Task<RequestResult> HttpGetFileAndCopyTo(string url, string fileName)
        {
            RequestResult innerResult = await HttpGetFileStream(url);
            if(!innerResult.IsSuccess())
                return RequestResult.ErrorFactory(innerResult.ResponseMessage, innerResult.Error);
            
            Console.Write($"──> 🔄️  Writing download result to: {fileName}");
            Stream stream = (Stream)innerResult.ParsedData!;
            await using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) 
            {
                await stream.CopyToAsync(fileStream);
            }
            Console.WriteLine($" └─> ✅ Downloaded File written to: {fileName}");
            return RequestResult.SuccessFactory(innerResult.ResponseMessage);
        }
        /// <summary>
        /// Performs a GET request to the given endpoint (baseUrl + url).
        /// This Method downloads the file and returns the contents as a string.
        /// </summary>
        /// <param name="url">The endpoint Url</param>
        /// <returns>The request result including the read contents as string.</returns>
        public async Task<RequestResult> HttpGetFileContents(string url)
        {
            RequestResult innerResult = await HttpGetFileStream(url);
            if (!innerResult.IsSuccess())
                return RequestResult.ErrorFactory(innerResult.ResponseMessage, innerResult.Error);
            string fileContents = await new StreamReader((Stream)innerResult.ParsedData!).ReadToEndAsync();
            return RequestResult.SuccessFactory(innerResult.ResponseMessage, fileContents);
        }
        private async Task<RequestResult> HttpGetFileStream(string url)
        {
            using (HttpClient client = BuildHttpClient())
            {
                HttpResponseMessage? response = null;
                try
                {
                    Console.WriteLine($"─> 🔄️ Performing File-Get request: {baseUrl + url}");
                    
                    response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    Stream stream = await response.Content.ReadAsStreamAsync();
                    Console.WriteLine($"└─> ✅ Downloaded with Response Status: {response.StatusCode} ");
                    return RequestResult.SuccessFactory(response, stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"└─> ❌ File-Get Request error: {e.Message}");
                    return RequestResult.ErrorFactory(response, e);
                }
            }
        }
        /// <summary>
        /// Performs a GET request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="options">The options for selecting the JSON DTO type.</param>
        /// <returns>The request result with parsed json data object</returns>
        public async Task<RequestResult> HttpGetJsonObject(string url, JsonDtoSelectionOptions options)
        {
            return await GetJsonFromRequestResult(await HttpGet(url), options);
        }
        /// <summary>
        /// Sends a request to _baseUrl + url and returns the string returned by that method.
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <returns>The returned string from the api and the request result.</returns>
        public async Task<RequestResult> HttpGetString(string url)
        {
            RequestResult response = await HttpGet(url);
            if (response.IsSuccess())
            {
                string data = await response.ResponseMessage!.Content.ReadAsStringAsync();
                return RequestResult.SuccessFactory(response.ResponseMessage, data);
            }
            return RequestResult.ErrorFactory(response.ResponseMessage, response.Error);
        }
        /// <summary>
        /// Performs a GET request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <returns>The request result with no parsed json data</returns>
        public async Task<RequestResult> HttpGet(string url)
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
        public async Task<RequestResult> HttpDelete(string url, HttpContent? content)
        {
            return await SendHttpRequest(HttpMethod.Delete, url, content);
        }
        /// <summary>
        /// Performs a DELETE request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult> HttpDeleteJson(string url, object? content)
        {
            return await HttpDelete(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a DELETE request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <param name="options">The options for selecting the JSON DTO type.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult> HttpDeleteJsonAndGetJson(string url, object? content, JsonDtoSelectionOptions options)
        {
            return await HttpDeleteContentAndGetJson(url, GetJsonHttpContentFromObject(content), options);
        }
        /// <summary>
        /// Performs a DELETE request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <param name="options">The options for selecting the JSON DTO type.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult> HttpDeleteContentAndGetJson(string url, HttpContent? content, JsonDtoSelectionOptions options)
        {
            return await GetJsonFromRequestResult(await HttpDelete(url, content), options);
        }
        
        // ----------------------------------------PUT----------------------------------------
        
        /// <summary>
        /// Performs a PUT request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The http content</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult> HttpPut(string url, HttpContent? content)
        {
            return await SendHttpRequest(HttpMethod.Put, url, content);
        }
        /// <summary>
        /// Performs a PUT request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult> HttpPutJson(string url, object? content)
        {
            return await HttpPut(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a PUT request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <param name="options">The options for selecting the JSON DTO type.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult> HttpPutJsonAndGetJson(string url, object? content, JsonDtoSelectionOptions options)
        {
            return await HttpPutContentAndGetJson(url, GetJsonHttpContentFromObject(content), options);
        }
        /// <summary>
        /// Performs a PUT request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <param name="options">The options for selecting the JSON DTO type.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult> HttpPutContentAndGetJson(string url, HttpContent? content, JsonDtoSelectionOptions options)
        {
            return await GetJsonFromRequestResult(await HttpPut(url, content), options);
        }
        
        // ----------------------------------------PATCH----------------------------------------
        
        /// <summary>
        /// Performs a PATCH request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The http content</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult> HttpPatch(string url, HttpContent? content)
        {
            return await SendHttpRequest(HttpMethod.Patch, url, content);
        }
        /// <summary>
        /// Performs a PATCH request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult> HttpPatchJson(string url, object? content)
        {
            return await HttpPatch(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a PATCH request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <param name="options">The options for selecting the JSON DTO type.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult> HttpPatchJsonAndGetJson(string url, object? content, JsonDtoSelectionOptions options)
        {
            return await HttpPatchContentAndGetJson(url, GetJsonHttpContentFromObject(content), options);
        }
        /// <summary>
        /// Performs a PATCH request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <param name="options">The options for selecting the JSON DTO type.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult> HttpPatchContentAndGetJson(string url, HttpContent content, JsonDtoSelectionOptions options)
        {
            return await GetJsonFromRequestResult(await HttpPatch(url, content), options);
        }
        
        // ----------------------------------------POST----------------------------------------
        
        /// <summary>
        /// Performs a POST request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The http content</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult> HttpPost(string url, HttpContent content)
        {
            return await SendHttpRequest(HttpMethod.Post, url, content);
        }
        /// <summary>
        /// Performs a POST request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <returns>The request result with no deserialized data.</returns>
        public async Task<RequestResult> HttpPostJson(string url, object? content)
        {
            return await HttpPost(url, GetJsonHttpContentFromObject(content));
        }
        /// <summary>
        /// Performs a POST request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <param name="options">The options for selecting the JSON DTO type.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult> HttpPostJsonAndGetJson(string url, object? content, JsonDtoSelectionOptions options)
        {
            return await HttpPostContentAndGetJson(url, GetJsonHttpContentFromObject(content), options);
        }
        /// <summary>
        /// Performs a POST request to the given endpoint (baseUrl + url)
        /// </summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="content">The object which should be serialized to json.</param>
        /// <param name="options">The options for selecting the JSON DTO type.</param>
        /// <returns>The request result with the deserialized object.</returns>
        public async Task<RequestResult> HttpPostContentAndGetJson(string url, HttpContent content, JsonDtoSelectionOptions options)
        {
            return await GetJsonFromRequestResult(await HttpPost(url, content), options);
        }

        private async Task<RequestResult> GetJsonFromRequestResult(RequestResult result, JsonDtoSelectionOptions options)
        {
            try
            {
                if(result.ResponseMessage is null)
                    return RequestResult.ErrorFactory(result.ResponseMessage, new Exception("Response message is null"));
                object? data = await HttpResultToJsonConverter.Convert(result.ResponseMessage, options);
                if (result.IsSuccess())
                    return RequestResult.SuccessFactory(result.ResponseMessage, data);
                return RequestResult.ErrorFactory(result.ResponseMessage, result.Error, data);
            }
            catch (Exception e)
            {
                return RequestResult.ErrorFactory(result.ResponseMessage, e);
            }
        }

        private async Task<RequestResult> SendHttpRequest(HttpMethod method, string url, HttpContent? content)
        {
            using (HttpClient client = BuildHttpClient())
            {
                HttpResponseMessage? response = null;
                try
                {
                    Console.WriteLine($"─> 🔄️ Performing {method.Method} request: {baseUrl + url}");
                    response = await client.SendAsync(new HttpRequestMessage(method, url){Content = content});
                    

                    // Ensure the response is successful
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($"└─> ✅ {method.Method} Response Status: {response.StatusCode} ");

                    return RequestResult.SuccessFactory(response);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"└─> ❌ {method.Method} Request error: {e.Message}");
                    return RequestResult.ErrorFactory(response, e);
                }
            }
        }
        private StringContent GetJsonHttpContentFromObject(object? objectToSerialize) => new StringContent(JsonSerializer.Serialize(objectToSerialize), Encoding.UTF8, "application/json");
        
        private HttpClient BuildHttpClient()
        {
            HttpClientBuilder builder; 
            if (Headers != null)
                builder = new HttpClientBuilder(baseUrl, Headers);
            else
                builder = new HttpClientBuilder(baseUrl);
            return builder.Build();
        }
    }
}
