using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace dotInstrukcije.Models
{
    public class InstructionSent
    {
        public string professorId { get; set; } = string.Empty;
        public DateTime date { get; set; }

    }
}
