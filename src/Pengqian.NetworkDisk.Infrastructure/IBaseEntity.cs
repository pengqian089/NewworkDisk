using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace Pengqian.NetworkDisk.Infrastructure
{
    public interface IBaseEntity
    {

    }

    public class BaseEntity : IBaseEntity
    {
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id { get; set; }
    }
}