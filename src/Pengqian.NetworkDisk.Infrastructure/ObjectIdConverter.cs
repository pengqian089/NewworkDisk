using System;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Pengqian.NetworkDisk.Infrastructure
{
    public class ObjectIdConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value != null ? value.ToString() : ObjectId.Empty.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value;
            if (objectType == typeof(ObjectId) && value != null)
            {
                return ObjectId.Parse(value.ToString());
            }
            return ObjectId.Empty;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ObjectId);
        }
    }
}