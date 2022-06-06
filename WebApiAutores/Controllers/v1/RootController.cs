using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> Get()
        {
            var isAdmin = await _authorizationService.AuthorizeAsync(User, "isAdmin");

            var dataHATEOAS = new List<DataHATEOAS>();

            dataHATEOAS.Add(new DataHATEOAS(link: Url.Link("GetRoot", new { }), description: "self", method: "GET"));

            dataHATEOAS.Add(new DataHATEOAS(link: Url.Link("getAuthors", new { }), description: "authors", method: "GET"));

            if(isAdmin.Succeeded)
            {
                dataHATEOAS.Add(new DataHATEOAS(link: Url.Link("createAuthor", new { }), description: "author-create", method: "POST"));

                dataHATEOAS.Add(new DataHATEOAS(link: Url.Link("newBook", new { }), description: "book-create", method: "POST"));
            }


            return Ok(dataHATEOAS);
        }
    }
}
