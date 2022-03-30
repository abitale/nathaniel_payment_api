using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PaymentApi.Configurations;
using PaymentApi.Models.DTOs.Requests;
using PaymentApi.Models.DTOs.Responses;

namespace PaymentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthManagementController(
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.email);
                if (existingUser != null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        errors = new List<string>() {
                        "Email already in use"
                    },
                        success = false
                    });
                }
            }

            var newUser = new IdentityUser() { Email = user.email, UserName = user.username };
            var isCreated = await _userManager.CreateAsync(newUser, user.password);
            if (isCreated.Succeeded)
            {
                var jwtToken = GenerateJwtToken(newUser);

                return Ok(new RegistrationResponse()
                {
                    success = true,
                    token = jwtToken
                });
            }
            else
            {
                return BadRequest(new RegistrationResponse()
                {
                    errors = isCreated.Errors.Select(x => x.Description).ToList(),
                    success = false
                });
            }
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDecriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDecriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.email);
                if (existingUser == null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        errors = new List<string>(){
                            "Invalid Login Request"
                        },
                        success = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.password);
                if (!isCorrect)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        errors = new List<string>() {
                        "Invalid login request"
                    },
                        success = false
                    });
                }

                var jwtToken = GenerateJwtToken(existingUser);

                return Ok(new RegistrationResponse()
                {
                    success = true,
                    token = jwtToken
                });
            }

            return BadRequest(new RegistrationResponse()
            {
                errors = new List<string>(){
                    "Invalid payload"
                },
                success = false
            });
        }
    }

}