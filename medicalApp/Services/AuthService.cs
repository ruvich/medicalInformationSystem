using BCrypt.Net;
using medicalApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace medicalApp.Services
{
    public class AuthService
    {
        private readonly string _secretKey;

        public AuthService(string secretKey)
        {
            _secretKey = secretKey;
        }

        public string hashPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                return hashedPassword;
            }
            else
            {
                return string.Empty;
            }
        }

        public bool verifyPassword(string password, string passwordHash)
        {
            if (!string.IsNullOrEmpty(password))
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            else
            {
                return false;
            }
        }

        public string generateJwtToken(DoctorModel doctor)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, doctor.id.ToString()),
                    new Claim(ClaimTypes.Authentication, Guid.NewGuid().ToString())
                    //new Claim(ClaimTypes.Email, doctor.email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = "MyClient",
                Issuer = "ApiToken"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}