# BoTech.HttpClientHelper

+ This project is a simple and efficient HTTP client helper library for .NET applications. It provides an easy-to-use interface for making HTTP requests and handling responses.

## Example
+ A simple example that shows how to use the NuGet package [BoTech.HttpClientHelper](https://www.nuget.org/packages/BoTech.HttpClientHelper/).
+ First, you obviously need to install the package:

```bash
dotnet add package BoTech.HttpClientHelper
```

+ And then you can use the package, for example, as follows:

```cs
using BoTech.HttpClientHelper;
using System.Text;

Console.WriteLine("Welcome to BoTech.HttpClientHelper. \n Testing some features...");

HttpRequestHelper httpRequestHelper = new HttpRequestHelper("https://example.com/");

RequestResult<string> result = httpRequestHelper.HttpGetString("").Result;

if(result.IsSuccess())
{
    Console.WriteLine("Result is: " + result.ParsedData);
}
```

### For more information please visit: [https://docs.botech.dev](https://docs.botech.dev/botech-httpclienthelper/v1-1/howtouse/)