using CRUD_Process.DTOs;
using CRUD_Process.Models;
using CRUD_Process.Repository;
using CRUD_Process.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Process.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly AuthService _authService;

        public AuthController(IRepository<User> userRepository, AuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        /// <summary>
        /// Register a new user (Admin or General User)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var existingUser = (await _userRepository.GetAll()).FirstOrDefault(u => u.Username == registerDto.Username);
            if (existingUser != null)
                return BadRequest("Username already exists");

            var newUser = new User
            {
                Username = registerDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password), // Encrypt Password
                Role = registerDto.Role
            };

            await _userRepository.Add(newUser);
            return Ok("User registered successfully");
        }

        /// <summary>
        /// Login a user and generate JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = (await _userRepository.GetAll()).FirstOrDefault(u => u.Username == loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
    }
}
