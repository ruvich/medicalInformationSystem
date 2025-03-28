using medicalApp.Data;
using medicalApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using medicalApp.Services;
using Microsoft.AspNetCore.Authorization;

namespace medicalApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Doctor : ControllerBase
    {
        private readonly AppDataContext _context;
        private readonly AuthService _authService;

        public Doctor(AppDataContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("login")]
        [Produces("text/plain", "application/json", "text/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCredentialsModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest("Invalid login request.");
                }

                var doctor = await _context.DoctorModels.FirstOrDefaultAsync(m => m.email == model.email);
                if (doctor == null || !_authService.verifyPassword(model.password, doctor.passwordHash))
                {
                    return Unauthorized("Invalid email or password.");
                }

                var token = _authService.generateJwtToken(doctor);

                var response = new TokenResponseModel
                {
                    Token = token
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [HttpPost("registration")]
        [Produces("text/plain", "application/json", "text/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] DoctorRegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid registration data.");
                }

                if (model.birthday >= DateTime.UtcNow)
                {
                    return BadRequest(new { status = "400", message = "Birthday cannot be in the future." });
                }

                var thisSpeciality = await _context.SpecialityModels.FirstOrDefaultAsync(s => s.id == model.speciality);
                if (thisSpeciality == null)
                {
                    return NotFound("Speciality not found.");
                }

                var doctor = new DoctorModel
                {
                    id = Guid.NewGuid(),
                    createTime = DateTime.UtcNow,
                    name = model.name,
                    birthday = model.birthday,
                    gender = model.gender,
                    email = model.email,
                    phone = model.phone,
                    passwordHash = _authService.hashPassword(model.password),
                    specialityId = model.speciality,
                    speciality = thisSpeciality,
                };

                _context.DoctorModels.Add(doctor);
                await _context.SaveChangesAsync();

                var token = _authService.generateJwtToken(doctor);

                var response = new TokenResponseModel
                {
                    Token = token
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            //try
            //{
            //    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //    if (string.IsNullOrEmpty(token))
            //    {
            //        return Unauthorized(new { status = "401", message = "Token is required." });
            //    }

            //    var invalidToken = new InvalidToken
            //    {
            //        id = Guid.NewGuid(),
            //        token = token,
            //        finish = DateTime.UtcNow.AddHours(1),
            //    };

            //    await _context.InvalidTokens.AddAsync(invalidToken);
            //    await _context.SaveChangesAsync();

            //    return Ok(new { status = "200", message = "Logged out successfully" });
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            //}
            return Ok(new { status = "Success", message = "Logged out successfully" });
        }

        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorProfileModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized(new { status = "401", message = "Unauthorized access" });
                }

                var doctor = await _context.DoctorModels.FirstOrDefaultAsync(m => m.id.ToString() == doctorId);

                if (doctor == null)
                {
                    return NotFound(new { status = "404", message = "Doctor not found" });
                }

                var doctorProfile = new DoctorProfileModel
                {
                    id = doctor.id,
                    createTime = doctor.createTime,
                    name = doctor.name,
                    email = doctor.email,
                    birthday = doctor.birthday,
                    gender = doctor.gender,
                    phone = doctor.phone
                };

                return Ok(doctorProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfile([FromBody] DoctorEditModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized();
                }

                var doctor = await _context.DoctorModels.FirstOrDefaultAsync(m => m.id.ToString() == doctorId);

                if (doctor == null)
                {
                    return NotFound();
                }

                doctor.email = model.email;
                doctor.name = model.name;
                doctor.birthday = model.birthday;
                doctor.gender = model.gender;
                doctor.phone = model.phone;

                _context.DoctorModels.Update(doctor);
                await _context.SaveChangesAsync();

                var updatedProfile = new DoctorProfileModel
                {
                    name = doctor.name,
                    email = doctor.email,
                    birthday = doctor.birthday,
                    gender = doctor.gender,
                    phone = doctor.phone
                };

                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }
    }
}