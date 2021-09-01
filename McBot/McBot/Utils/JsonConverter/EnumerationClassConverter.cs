using McBot.Core;

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McBot.Utils.JsonConverter
{
    public class EnumerationClassConverter<T> : JsonConverter<T> where T : EnumerationBase
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var val = reader.GetInt64();
            var possibleValues = EnumerationBase.GetAll<T>();
            var finalValue = possibleValues.Where(m => m.Id == val).Single();
            return finalValue;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Id);
        }

        //public override bool CanConvert(Type typeToConvert)
        //{
        //    return base.CanConvert(typeToConvert);
        //}

        //public override bool Equals(object obj)
        //{
        //    return base.Equals(obj);
        //}

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}

        //public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //{
        //    var val = reader.GetString();
        //    var possibleValues = EnumerationBase.GetAll<T>();
        //    var finalValue = possibleValues.Where(m => m.Id.ToString() == val).Single();
        //    return finalValue;
        //}

        //public override string ToString()
        //{
        //    return base.ToString();
        //}

        //public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        //{
        //    writer.WriteStringValue(value.Id.ToString());
        //}
        //public override T ReadJson(JsonReader reader, Type objectType, [AllowNull] T existingValue, bool hasExistingValue, JsonSerializer serializer)
        //{
        //    var val = reader.Value.ToString();

        //    var possibleValues = EnumerationBase.GetAll<T>();
        //    var finalValue = possibleValues.Where(m => m.Id.ToString() == val).Single();

        //    return finalValue;
        //}

        //public override void WriteJson(JsonWriter writer, [AllowNull] T value, JsonSerializer serializer)
        //{
        //    throw new NotImplementedException();
        //}
    }
}