using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McBot.Utils.JsonConverter
{
    public class IPAddresConverter : JsonConverter<IPAddress>
    {
        public override IPAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var val = reader.GetString();
            IPAddress Ip = IPAddress.Parse(val);
            return Ip;
        }

        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}