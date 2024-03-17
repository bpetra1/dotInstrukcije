using dotInstrukcije.Helper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using dotInstrukcije.Models;
using MongoDB.Bson;

namespace dotInstrukcije.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Student> _studentCollection;

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(
                mongoDbSettings.Value.ConnectionURL);
            var mongoDatabase = mongoClient.GetDatabase(
                mongoDbSettings.Value.DatabaseName);
            _studentCollection = mongoDatabase.GetCollection<Student>("Student");
        }

        public async Task<List<Student>> GetStudentsAsync() => await _studentCollection.Find(_ => true).ToListAsync();
        public async Task<Student> GetStudentAsyncById(ObjectId id) => await _studentCollection.Find(x => x.id == id).FirstOrDefaultAsync();
        public async Task CreateStudentAsync(Student student) => await _studentCollection.InsertOneAsync(student);
        public async Task UpdateStudentAsync(ObjectId id, Student student) => await _studentCollection.ReplaceOneAsync(x => x.id == id, student);
        public async Task DeleteStudentAsync(ObjectId id) => await _studentCollection.DeleteOneAsync(x => x.id == id);

    }
}
