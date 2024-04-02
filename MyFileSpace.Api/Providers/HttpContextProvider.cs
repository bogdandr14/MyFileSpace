using Microsoft.Extensions.Primitives;

namespace MyFileSpace.Api.Providers
{
    public class HttpContextProvider : IHttpContextProvider
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

        }

        public string GetValueFromRequestHeader(string key, string? defaultString = null)
        {
            if (_httpContextAccessor.HttpContext!.Request.Headers.TryGetValue(key, out StringValues headerValues) && headerValues.Any())
            {
                return headerValues.First()!;
            }

            return defaultString ?? String.Empty;
        }

        public int GetValueFromRequestHeader(string key, int defaultInt)
        {
            if (int.TryParse(GetValueFromRequestHeader(key), out int parsedInt))
            {
                return parsedInt;
            }

            return defaultInt;
        }

        public bool GetValueFromRequestHeader(string key, bool defaultBool)
        {
            if (bool.TryParse(GetValueFromRequestHeader(key), out bool parsedBool))
            {
                return parsedBool;
            }

            return defaultBool;
        }

        public TEnum GetValueFromRequestHeader<TEnum>(string key, TEnum defaultValue) where TEnum : Enum
        {
            if (Enum.TryParse(typeof(TEnum), GetValueFromRequestHeader(key), true, out object? parsedEnum))
            {
                return (TEnum)parsedEnum;
            }

            return defaultValue;
        }
    }
}
