using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Utilitarian.Migrations.Models
{
    public class MongoDbVersionRecord : VersionRecord
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
