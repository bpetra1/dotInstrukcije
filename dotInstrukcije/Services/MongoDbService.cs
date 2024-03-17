using dotInstrukcije.Helper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using dotInstrukcije.Models;
using MongoDB.Bson;
using System.Text;
using System.Security.Cryptography;

namespace dotInstrukcije.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Student> _studentCollection;
        private readonly IMongoCollection<Professor> _professorCollection;
        private readonly IMongoCollection<Subject> _subjectCollection;

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(
                mongoDbSettings.Value.ConnectionURL);
            var mongoDatabase = mongoClient.GetDatabase(
                mongoDbSettings.Value.DatabaseName);
            _studentCollection = mongoDatabase.GetCollection<Student>("Student");
            _professorCollection = mongoDatabase.GetCollection<Professor>("Professor");
            _subjectCollection = mongoDatabase.GetCollection<Subject>("Subject");

        }

        public async Task<List<Student>> GetStudentsAsync() => await _studentCollection.Find(_ => true).ToListAsync();
        public async Task<Student> GetStudentAsyncByEmail(string email) => await _studentCollection.Find(x => x.email == email).FirstOrDefaultAsync();
        public async Task<Student> GetStudentAsyncByEmailAndPassword(string email, string password)
        {
            string hashedPassword = HashPassword(password);
            return await _studentCollection.Find(x => x.email == email && x.password == hashedPassword).FirstOrDefaultAsync();
        }
        public async Task CreateStudentAsync(Student student) => await _studentCollection.InsertOneAsync(student);
        // public async Task UpdateStudentAsync(ObjectId id, Student student) => await _studentCollection.ReplaceOneAsync(x => x.id == id, student);
        // public async Task DeleteStudentAsync(ObjectId id) => await _studentCollection.DeleteOneAsync(x => x.id == id);

        public async Task<List<Professor>> GetProfessorsAsync() => await _professorCollection.Find(_ => true).ToListAsync();
        public async Task<Professor> GetProfessorAsyncByEmail(string email) => await _professorCollection.Find(x => x.email == email).FirstOrDefaultAsync();
        public async Task<Professor> GetProfessorAsyncByEmailAndPassword(string email, string password)
        {
            string hashedPassword = HashPassword(password);
            return await _professorCollection.Find(x => x.email == email && x.password == hashedPassword).FirstOrDefaultAsync();
        }

        public async Task CreateSubjectAsync(Subject subject) => await _subjectCollection.InsertOneAsync(subject);
        public async Task<List<Subject>> GetSubjectsAsync() => await _subjectCollection.Find(_ => true).ToListAsync();
        public async Task<Subject> GetSubjectAsyncByUrl(string url) => await _subjectCollection.Find(x => x.url == url).FirstOrDefaultAsync();
        public async Task<Subject> GetSubjectAsyncByTitle(string title) => await _subjectCollection.Find(x => x.title == title).FirstOrDefaultAsync();

        public async Task CreateProfessorAsync(Professor professor) => await _professorCollection.InsertOneAsync(professor);

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
