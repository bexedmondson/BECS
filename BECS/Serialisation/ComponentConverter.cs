using System.Text.Json;
using System.Text.Json.Serialization;

public class ComponentConverter : JsonConverter<IComponent>
{
    public override IComponent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //JsonSerializer.Deserialize(reader., typeToConvert, options);
        return null;
    }

    public override void Write(Utf8JsonWriter writer, IComponent value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                JsonSerializer.Serialize(writer, (IComponent) null, options);
                break;
            default:
            {
                var type = value.GetType();
                JsonSerializer.Serialize(writer, value, type, options);
                break;
            }
        }
    }
}
