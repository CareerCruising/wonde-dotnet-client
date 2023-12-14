using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Wonde.Wonde
{
    public class ObjectToArrayConverter : JsonConverter<IEnumerable<object>>
    {
        public override IEnumerable<object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<object>>(root.GetRawText(), options);
                }
                else
                {
                    var objList = new List<object> { JsonSerializer.Deserialize<Dictionary<string, object>>(root.GetRawText(), options) };
                    return objList;
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<object> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
