namespace MyFileSpace.Api.Providers
{
    public interface IHttpContextProvider
    {
        string GetValueFromRequestHeader(string key, string? defaultString = null);

        int GetValueFromRequestHeader(string key, int defaultInt);

        bool GetValueFromRequestHeader(string key, bool defaultBool);

        TEnum GetValueFromRequestHeader<TEnum>(string key, TEnum defaultEnum) where TEnum : Enum;

    }
}
