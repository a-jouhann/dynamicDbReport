using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicDbReport.DTO.Shared;

public static class Tools
{
    public static string ToJsonString(this object inputObject) => JsonSerializer.Serialize(inputObject);

    public static string ToJsonStringWithoutNull(this object inputObject) => JsonSerializer.Serialize(inputObject, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

    public static string ToJsonStringUnSafe(this object inputObject) => JsonSerializer.Serialize(inputObject, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

    public static T JsonToObject<T>(this string jsonObject, JsonConverter specialConvertor = null) where T : new()
    {
        if (string.IsNullOrEmpty(jsonObject) || string.IsNullOrWhiteSpace(jsonObject) || jsonObject == "\"") return new T();
        jsonObject = jsonObject.Replace("\r", "").Replace("\n", "");

        try
        {
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString };
            if (specialConvertor is not null) options.Converters.Add(specialConvertor);

            return JsonSerializer.Deserialize<T>(jsonObject, options);
        }
        catch (Exception)
        {
            return new T();
        }
    }
}
