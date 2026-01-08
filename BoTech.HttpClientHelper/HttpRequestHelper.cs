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
        public async Task<RequestResult<T>> HttpGetJsonObject<T>(string url)
        {
            RequestResult<dynamic> response = await HttpGet(url, null);
            if (response.IsSuccess())
            {
                string jsonData = await response.ResponseMessage!.Content.ReadAsStringAsync();
                return new RequestResult<T>(true, response.ResponseMessage, JsonConvert.DeserializeObject<T>(jsonData), null);
            }
            return new RequestResult<T>(false, response.ResponseMessage, default(T), response.Error);
        }
        public async Task<RequestResult<T>> HttpGetJsonObject<T>(string url, HttpContent? content)
        {
            RequestResult<dynamic> response = await HttpGet(url, content);
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
        /// <param name="url">The endpoint</param>
        /// <returns>The returned string from the api or the </returns>
        public async Task<RequestResult<string>> HttpGetString(string url, HttpContent? content)
        {
            RequestResult<dynamic> response = await HttpGet(url, content);
            if (response.IsSuccess())
            {
                string data = await response.ResponseMessage!.Content.ReadAsStringAsync();
                return new RequestResult<string>(true, response.ResponseMessage, data, null);
            }
            return new RequestResult<string>(false, response.ResponseMessage, string.Empty, response.Error);
        }
        /// <summary>
        /// Sends a request to _baseUrl + url and returns the http response.
        /// </summary>
        /// <param name="url">The Api url</param>
        /// <returns>The result of the Get request.</returns>
        public async Task<RequestResult<dynamic>> HttpGet(string url, HttpContent? content)
        {
            using (HttpClient client = BuildHttpClient())
            {
                HttpResponseMessage? response = null;
                try
                {
                    Console.Write($"Performing GET request: {_baseUrl + url}");
                    HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, url)
                    {
                        Content = content 
                    };
                    response = await client.SendAsync(httpRequest);

                    // Ensure the response is successful
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($" GET Response Status: {response.StatusCode} ");
                    return new RequestResult<dynamic>(true, response, null, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine($" GET Request error: {e.Message}");
                    return new RequestResult<dynamic>(false, response, null, e);
                }
            }
        }
        public async Task<RequestResult<dynamic>> HttpPatch(string url, HttpContent? content)
        {
            using (HttpClient client = BuildHttpClient())
            {
                HttpResponseMessage? response = null;
                try
                {
                    Console.Write($"Performing Patch request: {_baseUrl + url}");
                    response = await client.PatchAsync(url, content);

                    // Ensure the response is successful
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($" Patch Response Status: {response.StatusCode} ");
                    return new RequestResult<dynamic>(true, response, null, null);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($" Patch Request error: {e.Message}");
                    return new RequestResult<dynamic>(false, response, null, e);
                }
            }
        }
        public async Task<RequestResult<dynamic>> HttpPost(string url, HttpContent content)
        {
            using (HttpClient client = BuildHttpClient())
            {
                HttpResponseMessage? response = null;
                try
                {
                    Console.Write($"Performing Post request: {_baseUrl + url}");
                    response = await client.PostAsync(url, content);
                    

                    // Ensure the response is successful
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($" Patch Response Status: {response.StatusCode} ");

                    return new RequestResult<dynamic>(true, response, null, null);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($" Patch Request error: {e.Message}");
                    return new RequestResult<dynamic>(false, response, null, e);
                }
            }
        }
        public async Task<RequestResult<dynamic>> HttpPostJson(string url, object? content)
        {
            if(content != null)
                return await HttpPost(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            else 
                return await HttpPost(url, new StringContent(string.Empty));
        }

        public async Task<RequestResult<T>> HttpPostJsonAndGetJson<T>(string url, object? content)
        {
            try
            {
                RequestResult<dynamic> result = await HttpPostJson(url, content);
                if (result.IsSuccess())
                {
                    string jsonData = await result.ResponseMessage!.Content.ReadAsStringAsync();
                    if (jsonData.Length > 0)
                        return new RequestResult<T>(true, result.ResponseMessage,
                            JsonConvert.DeserializeObject<T>(jsonData), null);
                }

                return new RequestResult<T>(false, result.ResponseMessage, default(T), result.Error);
            }
            catch (Exception e)
            {
                return new RequestResult<T>(false, null, default(T), e);
            }
        }

        public async Task<RequestResult<T>> HttpPostGetJson<T>(string url)
        {
            return await HttpPostJsonAndGetJson<T>(url, null);
        }
        private HttpClient BuildHttpClient()
        {
            HttpClientBuilder builder = new HttpClientBuilder(_baseUrl);
            if (Headers != null)
            {
                builder = new HttpClientBuilder(_baseUrl, Headers);
            }
            return builder.Build();
        }
    }
}
