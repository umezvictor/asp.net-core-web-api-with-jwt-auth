using FileManagerApi.Data;
using FileManagerApi.Dtos;
using FileManagerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerApi.Controllers
{
    //api/auth
    [Route("api/[controller]")]
    [ApiController] //this also enables validation from model or wherever to show on api call. 
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        //
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        //dto are used to map models to objects
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //valdate request

            //convert username to lowercase for consistency
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            //check if username is taken using the usderExists method create earlier in repo
            //userforregisterdto infers to the request body
            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            //create an instance of User class
            var userToCreate = new User
            {
                //set username to user input
                Username = userForRegisterDto.Username
            };

            //create user
            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);


            return StatusCode(201);
        }


        //login user
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //check if user exists
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            //build up a token to be sent to user
            //contains id and usernamae

            //token doesn't need to check database to validate

            //set claims -- our token has two claims

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //key to sign token
            //encode it to a byte array using argument
            //this key will be stored in app settings, just like database connection string
            //hence bring in Iconfiguration --di
            //AppSettings:Token  should be created in appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //generate signin credentials -- takes in key and algorithm

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //create security token descriptor

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), //expires after 1 day --24hrs
                SigningCredentials = creds
            };

            //token handler

            var tokenHandler = new JwtSecurityTokenHandler();

            //using token handler, we can create token and pass in token descriptor

            var token = tokenHandler.CreateToken(tokenDescriptor);//contains token to be senbt to client

            //return token to client as object
            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });

        }
        
    }
}