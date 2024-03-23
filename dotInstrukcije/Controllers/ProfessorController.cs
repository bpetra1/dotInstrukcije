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
                var subjects = formData["subjects"];

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

            var role = "Professor";
            var token = _jwtTokenService.GenerateToken(model.Email, role);

            return Ok(new
            {
                success = true,
                professor = professor,
                token = token,
                message = "Login successful"
            });
        }

        [HttpPost("/updateNameSurname")]
        [Authorize]
        public async Task<IActionResult> UpdateNameSurname([FromBody] UpdateNameSurname model)
        {
            string email = User.FindFirst(ClaimTypes.Name)?.Value;
            string role = User.FindFirst(ClaimTypes.Role)?.Value;

            if(role == "Professor")
            {
                try
                {
                    var professorToUpdate = await _mongodbService.GetProfessorAsyncByEmail(email);
                    string confirmPassword = _mongodbService.HashPassword(model.password);

                    if (confirmPassword == professorToUpdate.password)
                    {
                        professorToUpdate.name = model.name;
                        professorToUpdate.surname = model.surname;

                        await _mongodbService.UpdateProfessorAsync(professorToUpdate, email);

                        return Ok(new { success = true, message = "Professor found successfully" });
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Wrong password." });
                    }

                } catch(Exception ex)
                {
                    return BadRequest(ex);
                }

            }
            else if(role =="Student")
            {
                try
                {
                    var studentToUpdate = await _mongodbService.GetStudentAsyncByEmail(email);

                    string confirmPassword = _mongodbService.HashPassword(model.password);

                    if (confirmPassword == studentToUpdate.password)
                    {
                        studentToUpdate.name = model.name;
                        studentToUpdate.surname = model.surname;

                        await _mongodbService.UpdateStudentAsync(studentToUpdate, email);

                        return Ok(new { success = true, message = "Student found successfully" });
                    }
                    else
                    {
                        return BadRequest(new { success=false, message = "Wrong password." });
                    }
                } catch(Exception ex)
                {
                    return BadRequest($"{ex.Message}");
                }

            }
            else
            {
                return NotFound();
            }


        }

        
        [HttpPost("/updateProfilePicture")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePicture([FromBody] UpdateProfilePicture model)
        {
            string email = User.FindFirst(ClaimTypes.Name)?.Value;
            string role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Professor")
            {
                try
                {
                    var professorToUpdate = await _mongodbService.GetProfessorAsyncByEmail(email);
                    string confirmPassword = _mongodbService.HashPassword(model.password);

                    if (confirmPassword == professorToUpdate.password)
                    {
                        professorToUpdate.profilePictureUrl = model.profilePictureUrl;

                        await _mongodbService.UpdateProfessorAsync(professorToUpdate, email);

                        return Ok(new { success = true, message = "Professor found successfully" });
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Wrong password." });
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

            }
            else if (role == "Student")
            {
                try
                {
                    var studentToUpdate = await _mongodbService.GetStudentAsyncByEmail(email);

                    string confirmPassword = _mongodbService.HashPassword(model.password);

                    if (confirmPassword == studentToUpdate.password)
                    {
                        studentToUpdate.profilePictureUrl= model.profilePictureUrl;

                        await _mongodbService.UpdateStudentAsync(studentToUpdate, email);

                        return Ok(new { success = true, message = "Student found successfully" });
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Wrong password." });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"{ex.Message}");
                }

            }
            else
            {
                return NotFound();
            }


        }

        [HttpPost("/updatePassword")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePassword model)
        {
            string email = User.FindFirst(ClaimTypes.Name)?.Value;
            string role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Professor")
            {
                try
                {
                    var professorToUpdate = await _mongodbService.GetProfessorAsyncByEmail(email);
                    string confirmPassword = _mongodbService.HashPassword(model.password);

                    if (confirmPassword == professorToUpdate.password)
                    {
                        var newPass = _mongodbService.HashPassword(model.newpassword);
                        professorToUpdate.password = newPass;

                        await _mongodbService.UpdateProfessorAsync(professorToUpdate, email);

                        return Ok(new { success = true, message = "Professor found successfully" });
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Wrong password." });
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

            }
            else if (role == "Student")
            {
                try
                {
                    var studentToUpdate = await _mongodbService.GetStudentAsyncByEmail(email);

                    string confirmPassword = _mongodbService.HashPassword(model.password);

                    if (confirmPassword == studentToUpdate.password)
                    {
                        var newPass = _mongodbService.HashPassword(model.newpassword);
                        studentToUpdate.password = newPass;

                        await _mongodbService.UpdateStudentAsync(studentToUpdate, email);

                        return Ok(new { success = true, message = "Student found successfully" });
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Wrong password." });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"{ex.Message}");
                }

            }
            else
            {
                return NotFound();
            }


        }


    }


}



