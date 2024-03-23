using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace dotInstrukcije.Models
{
    public class InstructionsDate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId id { get; set; }
        public string studentId { get; set; }
        public string email { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; } = string.Empty;

    }
}
