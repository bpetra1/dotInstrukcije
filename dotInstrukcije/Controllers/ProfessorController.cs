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
    public class ProfessorController : Controller
    {
        private readonly MongoDbService _mongodbService;
        private readonly JwtTokenService _jwtTokenService;

        public ProfessorController(MongoDbService mongoDbService, JwtTokenService jwtTokenService)
        {
            _mongodbService = mongoDbService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("professors")]
        [Authorize]
        public async Task<IActionResult> GetProfessors()
        {
            try
            {
                var professors = await _mongodbService.GetProfessorsAsync();
                return Ok(new { success = true, professors, message = "Retrieved professors successfully" });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
        [HttpGet("professor/{email}")]
        [Authorize]
        public async Task<IActionResult> GetProfessor(string email)
        {
            var professor = await _mongodbService.GetProfessorAsyncByEmail(email);
            if (professor == null)
            {
                return NotFound(new { success = false, message = "Professor not found" });
            }

            return Ok(new { success = true, professor, message = "Professor found successfully" });
        }

        [HttpPost("/register/professor")]
        public async Task<IActionResult> RegisterProfessor([FromForm] IFormCollection formData)
        {
            try
            {
                var email = formData["email"];
                var name = formData["name"];
                var surname = formData["surname"];
                var password = formData["password"];
                var confirmPassword = formData["confirmPassword"];
                var profilePictureUrl = formData["profilePictureUrl"];
                var subjects = formData["subjects"]; // Assuming subjects are sent as an array

                if (password != confirmPassword)
                {
                    return BadRequest(new { success = false, message = "Password and confirm password do not match" });
                }

                var existingProfessor = await _mongodbService.GetProfessorAsyncByEmail(email);
                if (existingProfessor != null)
                {
                    return Conflict(new { success = false, message = "Email already exists" });
                }

                var professor = new Professor
                {
                    email = email,
                    name = name,
                    surname = surname,
                    password = _mongodbService.HashPassword(password),
                    profilePictureUrl = profilePictureUrl,
                    subjects = subjects.ToList() 
                };

                await _mongodbService.CreateProfessorAsync(professor);

                return CreatedAtAction(nameof(GetProfessor), new { email = professor.email }, new { success = true, message = "Professor registered successfully" });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("/login/professor")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null)
            {
                return BadRequest(new { success = false, message = "Invalid request data" });
            }

            var professor = await _mongodbService.GetProfessorAsyncByEmailAndPassword(model.Email, model.Password);
            if (professor == null)
            {
                return BadRequest(new { success = false, message = "Invalid email or password" });
            }

            var token = _jwtTokenService.GenerateToken(professor.email);

            return Ok(new
            {
                success = true,
                professor = professor,
                token = token,
                message = "Login successful"
            });
        }


    }
}
