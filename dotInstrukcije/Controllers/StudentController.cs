using dotInstrukcije.Services;
using dotInstrukcije.Models;
using MongoDB.Bson;

using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.IdGenerators;

namespace dotInstrukcije.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : Controller
    {
        private readonly MongoDbService _mongodbService;
        public StudentController(MongoDbService mongoDbService) => _mongodbService = mongoDbService;

        [HttpGet]
        public async Task<List<Student>> Get() => await _mongodbService.GetStudentsAsync();

        [HttpGet("Get/{id}")]
        public async Task<Student> Get(ObjectId id)
        {
            var student = await _mongodbService.GetStudentAsyncById(id);
            if(student is null)
            {
                return null;
            }

            return student;
        }

        [HttpPost("CreateAsync")]
        public async Task<IActionResult> CreateAsync(Student student)
        {
            await _mongodbService.CreateStudentAsync(student);

            return CreatedAtAction(nameof(Get), new { id = student.id }, student);
        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(ObjectId id, Student student)
        {
            var studentToUpdate = await _mongodbService.GetStudentAsyncById(id);

            if(studentToUpdate is null)
            {
                return NotFound();
            }

            student.id = studentToUpdate.id;

            await _mongodbService.UpdateStudentAsync(id, student);

            return NoContent();
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(ObjectId id)
        {
            var student = await _mongodbService.GetStudentAsyncById(id);

            if (student is null)
            {
                return NotFound();
            }

            await _mongodbService.DeleteStudentAsync(id);

            return NoContent();
        }

    }
}
