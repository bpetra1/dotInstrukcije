using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace dotInstrukcije.Models
{
    public class Professor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string password { get; set; }
        public string profilePictureUrl { get; set; }
        public int instructionsCount { get; set; } = 0;
        public List<string> subjects { get; set; }
    }
}
