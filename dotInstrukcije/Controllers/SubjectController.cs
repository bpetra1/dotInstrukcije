using dotInstrukcije.Services;
using dotInstrukcije.Models;
using MongoDB.Bson;

using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.IdGenerators;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace dotInstrukcije.Controllers
{
    [ApiController]
    public class SubjectController : Controller
    {
        private readonly MongoDbService _mongodbService;
        private readonly JwtTokenService _jwtTokenService;

        public SubjectController(MongoDbService mongoDbService, JwtTokenService jwtTokenService)
        {
            _mongodbService = mongoDbService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects()
        {
            try
            {
                var subjects = await _mongodbService.GetSubjectsAsync();
                return Ok(new { success = true, subjects, message = "Retrieved subjects successfully" });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
        [HttpGet("subject/{url}")]
        [Authorize]
        public async Task<IActionResult> GetSubject(string url)
        {
            var subject = await _mongodbService.GetSubjectAsyncByUrl(url);
            if (subject == null)
            {
                return NotFound(new { success = false, message = "Subject not found" });
            }

            return Ok(new { success = true, subject, message = "Subject found successfully" });
        }

        [HttpPost("/subject")]
        [Authorize]
        public async Task<IActionResult> CreateSubject(Subject subject)
        {
            try
            {
                var existingSubjectByUrl = await _mongodbService.GetSubjectAsyncByUrl(subject.url);
                var existingSubjectByTitle = await _mongodbService.GetSubjectAsyncByTitle(subject.title);

                if (existingSubjectByUrl != null)
                {
                    return Conflict(new { success = false, message = "Subject with the same URL already exists" });
                }
                else if (existingSubjectByTitle != null)
                {
                    return Conflict(new { success = false, message = "Subject with the same title already exists" });
                }

                await _mongodbService.CreateSubjectAsync(subject);
                return CreatedAtAction(nameof(GetSubject), new { url = subject.url }, new { success = true, subject, message = "Subject created successfully" });

            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("instructions")]
        [Authorize] 
        public async Task<IActionResult> ScheduleInstructionSession([FromBody] InstructionsDate request)
        {
            try
            {
                var studentId = User.FindFirst(ClaimTypes.Email)?.Value;

                request.studentId = studentId;
                request.status = "sentInstructionRequests";

                var success = await _mongodbService.ScheduleInstructionSessionAsync(request);

                if (success)
                {
                    return Ok(new { success = true, message = "Instruction session scheduled successfully" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to schedule instruction session" });
                }
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }


    }
}
