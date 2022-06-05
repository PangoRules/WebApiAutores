using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountsController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._configuration = configuration;
        }

        [HttpPost("register")] //POST: api/accounts/register
        public async Task<ActionResult<AuthResponseDto>> Register(UserCredentialsDto userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };

            var result = await _userManager.CreateAsync(user, userCredentials.Password);

            if(result.Succeeded)
            {
                return Ok(CreateToken(userCredentials));
            }
            else
                return BadRequest(result.Errors);
        }

        [HttpPost("login")] //POST: api/accounts/login
        public async Task<ActionResult<AuthResponseDto>> Login(UserCredentialsDto userCredentials)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if(result.Succeeded)
            {
                return Ok(CreateToken(userCredentials));
            }
            else
                return BadRequest("Unsuccessful login");
        }

        [HttpGet("RefreshToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<AuthResponseDto> RefreshToken()
        {
            var userCredentials = new UserCredentialsDto()
            {
                Email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value
            };

            return Ok(CreateToken(userCredentials));
        }
        
        /// <summary>
        /// Function in charge of creating the Jwt-Token.
        /// </summary>
        /// <param name="userCredentials">Credentials user has to pass to create the token.</param>
        /// <returns>An object with the token and the expiring date of it inside.</returns>
        private AuthResponseDto CreateToken(UserCredentialsDto userCredentials)
        {
            //Creating the list of claims visible to the user (Also in the UI)
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email)
            };

            //Getting the encryption key for the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]));

            //Creating the credentials to sign-in credentials from the key before
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Expire date for the token
            var expireDate = DateTime.UtcNow.AddHours(1);

            //Creating the token, search for this in Google for more info
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expireDate, signingCredentials: creds);

            return new AuthResponseDto()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expireDate,
            };
        }
    }
}
