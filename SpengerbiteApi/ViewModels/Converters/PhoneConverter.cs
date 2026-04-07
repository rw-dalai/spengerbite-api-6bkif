using System.Text.Json;
using System.Text.Json.Serialization;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.ViewModels.Converters;

public class PhoneConverter : JsonConverter<Phone>
{
    public override Phone Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(reader.GetString());

    public override void Write(Utf8JsonWriter writer, Phone phone, JsonSerializerOptions options)
    {
        writer.WriteStringValue(phone.Value);
    }
}
