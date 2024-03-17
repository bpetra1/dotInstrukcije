using dotInstrukcije.Services;
using dotInstrukcije.Models;
using MongoDB.Bson;

using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.IdGenerators;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace dotInstrukcije.Controllers
{
    [ApiController]
    public class StudentController : Controller
    {
        private readonly MongoDbService _mongodbService;
        private readonly JwtTokenService _jwtTokenService;

        public StudentController(MongoDbService mongoDbService, JwtTokenService jwtTokenService)
        {
            _mongodbService = mongoDbService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("students")]
        [Authorize]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                var students = await _mongodbService.GetStudentsAsync();
                return Ok(new { success = true, students, message = "Retrieved students successfully" });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
        [HttpGet("student/{email}")]
        [Authorize]
        public async Task<IActionResult> GetStudent(string email)
        {
            var student = await _mongodbService.GetStudentAsyncByEmail(email);
            if (student == null)
            {
                return NotFound(new { success = false, message = "Student not found" });
            }

            return Ok(new { success = true, student, message = "Student found successfully"});
        }

        [HttpPost("/register/student")]
        public async Task<IActionResult> RegisterStudent(Student student)
        {
            try
            {
                var existingStudent = await _mongodbService.GetStudentAsyncByEmail(student.email);
                if (existingStudent != null)
                {
                    return Conflict(new { success = false, message = "Email already exists" });
                }

                student.password = _mongodbService.HashPassword(student.password);

                await _mongodbService.CreateStudentAsync(student);

                return CreatedAtAction(nameof(GetStudent), new { email = student.email }, new { success = true, message = "Student registered successfully" });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("/login/student")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var student = await _mongodbService.GetStudentAsyncByEmailAndPassword(model.Email, model.Password);
            if (student == null)
            {
                return BadRequest(new { success = false, message = "Invalid email or password" });
            }

            var token = _jwtTokenService.GenerateToken(student.email);

            return Ok(new
            {
                success = true,
                student = student,
                token = token,
                message = "Login successful"
            });

        }


        /*
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
        */

    }
}
