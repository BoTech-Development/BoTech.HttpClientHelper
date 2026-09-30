using System.Net;
using BoTech.HttpClientHelper.Models;
using System.Text.Json;

namespace BoTech.HttpClientHelper.Services
{
    internal static class HttpResultToJsonConverter
    {
        public static async Task<object?> Convert(HttpResponseMessage serverResponse, JsonDtoSelectionOptions options)
        {
            string jsonData = await serverResponse.Content.ReadAsStringAsync();
            if (jsonData.Length == 0)
                return null;
            if (options.EndpointOnlyReturnsDtoIfStatusOk)
                return StandardJsonParsing(jsonData, options.DefaultOkDtoType!);
            else
                return JsonDeserializationWithHttpStatusCodeSelection(jsonData, options, serverResponse.StatusCode);
        }

        private static object? JsonDeserializationWithHttpStatusCodeSelection(string jsonData, JsonDtoSelectionOptions options, HttpStatusCode status)
        {
            if(options.DtoTypeForEachHttpStatusCode.TryGetValue(status, out var targetDtoType))
            {
                return JsonSerializer.Deserialize(jsonData, targetDtoType, JsonSerializerOptions.Web);
            }
            return null;
        }
        private static object? StandardJsonParsing(string jsonData, Type targetDtoType)
        {
            return JsonSerializer.Deserialize(jsonData, targetDtoType, JsonSerializerOptions.Web);
        }
    }
}