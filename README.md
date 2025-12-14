# BoTech.HttpClientHelper

+ This project is a simple and efficient HTTP client helper library for .NET applications. It provides an easy-to-use interface for making HTTP requests and handling responses.

## Example
+ A simple example of to use the BoTech.HttpClientHelper library with the github api.

```cs
using BoTech.HttpClientHelper;
using System.Text;

Console.WriteLine("Welcome to BoTech.HttpClientHelper. \n Testing some features...");

HttpRequestHelper httpRequestHelper = new HttpRequestHelper("https://api.github.com");

RequestResult<string> result = httpRequestHelper.HttpGetString(
    "/markdown", 
    new StringContent("{\"text\":\"Hello **world**\"}", Encoding.UTF8, "application/json")).Result;

if(result.IsSuccess())
{
    Console.WriteLine("Result is: " + result.ParsedData);
}
```