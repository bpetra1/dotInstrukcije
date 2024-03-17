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
        public async Task<IActionResult> RegisterProfessor(Professor professor)
        {
            try
            {
                var existingProfessor = await _mongodbService.GetProfessorAsyncByEmail(professor.email);
                if (existingProfessor != null)
                {
                    return Conflict(new { success = false, message = "Email already exists" });
                }

                professor.password = _mongodbService.HashPassword(professor.password);

                await _mongodbService.CreateProfessorAsync(professor);

                return CreatedAtAction(nameof(GetProfessor), new { email = professor.email }, new { success = true, message = "Professor registered successfully" });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("/login/professor")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
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
