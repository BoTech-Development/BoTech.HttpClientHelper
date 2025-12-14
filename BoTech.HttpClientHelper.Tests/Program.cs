// See https://aka.ms/new-console-template for more information
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