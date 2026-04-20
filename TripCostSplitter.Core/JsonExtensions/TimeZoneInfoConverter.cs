using System.Text.Json;
using System.Text.Json.Serialization;

namespace TripCostSplitter.Core.JsonExtensions;

public class TimeZoneInfoConverter: JsonConverter<TimeZoneInfo>
{
    public override TimeZoneInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? timeZoneText = reader.GetString();
        if (string.IsNullOrEmpty(timeZoneText))
        {
            return TimeZoneInfo.Local;
        }
        
        try
        {
            return TimeZoneInfo.CreateCustomTimeZone(
                timeZoneText, TimeSpan.Parse(timeZoneText), timeZoneText, timeZoneText);
        }
        catch (Exception)
        {
            return TimeZoneInfo.Local;
        }
    }
    
    public override void Write(Utf8JsonWriter writer, TimeZoneInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.BaseUtcOffset.ToString());
    }
}