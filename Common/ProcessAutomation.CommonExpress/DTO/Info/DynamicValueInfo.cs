using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// IT ALLOWS A JSON PROPERTY TO BE HANDLED AS AN OBJECT OR AS ANOTHER DYNAMIC TYPE.
/// </summary>
public class DynamicValueConverter : JsonConverter<DynamicValue>
{
    public override void WriteJson(JsonWriter writer, DynamicValue? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteValue(value.Token.ToString(Formatting.None));
    }

    public override DynamicValue? ReadJson(JsonReader reader, Type objectType, DynamicValue? existingValue,
                                        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var token = JToken.Load(reader);

        return new DynamicValue(token);
    }
}

[JsonConverter(typeof(DynamicValueConverter))]
public class DynamicValue
{
    private readonly JToken token;
    public JToken Token => token;
    public JTokenType Type => token.Type;

    public bool IsString => Type == JTokenType.String;
    public bool IsObject => Type == JTokenType.Object;
    public bool IsArray => Type == JTokenType.Array;
    public bool IsNull => Type == JTokenType.Null;

    public DynamicValue(JToken token)
    {
        this.token = token ?? JValue.CreateNull();
    }

    public string? GetString()
    {
        return IsString ? token.Value<string>() : token.ToString(Formatting.Indented);
    }

    public T? Get<T>()
    {
        return token.ToObject<T>();
    }

    public override string ToString()
    {
        return GetString() ?? string.Empty;
    }

    public static implicit operator string?(DynamicValue value)
    {
        return value?.GetString();
    }
}