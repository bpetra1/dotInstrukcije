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
    //[Authorize]
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


    }
}
