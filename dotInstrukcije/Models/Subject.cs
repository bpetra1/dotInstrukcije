using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace dotInstrukcije.Models
{
    public class Subject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string description { get; set; }
    }
}
