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
        public async Task<IActionResult> RegisterStudent([FromForm] IFormCollection formData)
        {
            try
            {
                var email = formData["email"];
                var name = formData["name"];
                var surname = formData["surname"];
                var password = formData["password"];
                var confirmPassword = formData["confirmPassword"];
                var profilePictureUrl = formData["profilePictureUrl"];

                if (password != confirmPassword)
                {
                    return BadRequest(new { success = false, message = "Password and confirm password do not match" });
                }

                var existingStudent = await _mongodbService.GetStudentAsyncByEmail(email);
                if (existingStudent != null)
                {
                    return Conflict(new { success = false, message = "Email already exists" });
                }

                var student = new Student
                {
                    email = email,
                    name = name,
                    surname=surname,
                    password = _mongodbService.HashPassword(password),
                    profilePictureUrl = profilePictureUrl
                };


                await _mongodbService.CreateStudentAsync(student);

                return CreatedAtAction(nameof(GetStudent), new { email = student.email }, new { success = true, message = "Student registered successfully" });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("/login/student")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null)
            {
                return BadRequest(new { success = false, message = "Invalid request data" });
            }

            var student = await _mongodbService.GetStudentAsyncByEmailAndPassword(model.Email, model.Password);
            if (student == null)
            {
                return BadRequest(new { success = false, message = "Invalid email or password" });
            }

            var token = _jwtTokenService.GenerateToken();

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
