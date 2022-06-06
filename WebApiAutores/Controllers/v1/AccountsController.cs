using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;
using WebApiAutores.Services;

namespace WebApiAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountsController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register", Name = "newUser")] //POST: api/accounts/register
        public async Task<ActionResult<AuthResponseDto>> Register(UserCredentialsDto userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };

            var result = await _userManager.CreateAsync(user, userCredentials.Password);

            if(result.Succeeded)
            {
                return Ok(await CreateToken(userCredentials));
            }
            else
                return BadRequest(result.Errors);
        }

        [HttpPost("login", Name = "loginUser")] //POST: api/accounts/login
        public async Task<ActionResult<AuthResponseDto>> Login(UserCredentialsDto userCredentials)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if(result.Succeeded)
            {
                return Ok(await CreateToken(userCredentials));
            }
            else
                return BadRequest("Unsuccessful login");
        }

        [HttpGet("RefreshToken", Name = "refreshToken")] //GET: api/accounts/RefreshToken
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken()
        {
            var userCredentials = new UserCredentialsDto()
            {
                Email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value
            };

            return Ok(await CreateToken(userCredentials));
        }

        [HttpPost("MakeAdmin", Name = "makeAdmin")] //POST: api/accounts/MakeAdmin
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> MakeAdmin(EditAdminDto editAdminDto)
        {
            var user = await _userManager.FindByEmailAsync(editAdminDto.Email);

            var claimsList = await _userManager.GetClaimsAsync(user);

            if(claimsList.FirstOrDefault(c => c.Type == "isAdmin") != null)
                return BadRequest("User is already admin.");

            await _userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));

            return NoContent();
        }

        [HttpPost("RemoveAdmin", Name = "removeAdmin")] //POST: api/accounts/MakeAdmin
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> RemoveAdmin(EditAdminDto editAdminDto)
        {
            var user = await _userManager.FindByEmailAsync(editAdminDto.Email);

            var claimsList = await _userManager.GetClaimsAsync(user);

            if(claimsList.FirstOrDefault(c => c.Type == "isAdmin") == null)
                return BadRequest("User doesn't have admin privileges to remove.");

            await _userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1"));

            return NoContent();
        }

        /// <summary>
        /// Function in charge of creating the Jwt-Token.
        /// </summary>
        /// <param name="userCredentials">Credentials user has to pass to create the token.</param>
        /// <returns>An object with the token and the expiring date of it inside.</returns>
        private async Task<AuthResponseDto> CreateToken(UserCredentialsDto userCredentials)
        {
            //Creating the list of claims visible to the user (Also in the UI)
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email)
            };

            //Getting the user if is already created to find it's list of claims
            var user = await _userManager.FindByEmailAsync(userCredentials.Email);
            if(user != null)
            {
                var claimsDb = await _userManager.GetClaimsAsync(user);

                claims.AddRange(claimsDb);
            }


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
