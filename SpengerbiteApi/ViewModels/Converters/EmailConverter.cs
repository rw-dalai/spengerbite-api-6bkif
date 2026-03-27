using System.Text.Json;
using System.Text.Json.Serialization;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.ViewModels.Converters;

public class EmailConverter : JsonConverter<Email>
{
    // De-serialization
    public override Email Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => 
        new(reader.GetString());

    // Serialization
    public override void Write(Utf8JsonWriter writer, Email email, JsonSerializerOptions options)
    {
        writer.WriteStringValue(email.Value);
    }
}