using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BoTech.HttpClientHelper.Models
{
    public class JsonDtoSelectionOptions
    {
        /// <summary>
        /// When the server responses with 200 OK, this type will be used to deserialize the response content, when the property is not null.
        /// </summary>
        public Type? DefaultOkDtoType { get; init; } = null;

        /// <summary>
        /// When the server responses with different dto's for different status codes, this dictionary will be used to select the correct dto type for deserialization.
        /// The Dictionary is empty when <see cref="DefaultOkDtoType"/> is not null / used.
        /// </summary>
        public Dictionary<HttpStatusCode, Type> DtoTypeForEachHttpStatusCode { get; init; } =
            new Dictionary<HttpStatusCode, Type>();

        public bool EndpointOnlyReturnsDtoIfStatusOk => DefaultOkDtoType != null && DtoTypeForEachHttpStatusCode.Count == 0;

        public static JsonDtoSelectionOptions CreateForSingleStatusOkDtoType(Type dtoType)
        {
            return new JsonDtoSelectionOptions()
            {
                DefaultOkDtoType = dtoType
            };
        }
    }
}
