using BoTech.HttpClientHelper;
using System.Text;

Console.WriteLine("Welcome to BoTech.HttpClientHelper. \n Testing some features...");

HttpRequestHelper httpRequestHelper = new HttpRequestHelper("https://example.com/");

RequestResult<string> result = httpRequestHelper.HttpGetString("").Result;

if(result.IsSuccess())
{
    Console.WriteLine("Result is: " + result.ParsedData);
}