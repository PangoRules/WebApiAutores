using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiAutores.DTOs;

namespace WebApiAutores.Services
{
    public class LinkGenerator
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LinkGenerator(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor)
        {
            this._authorizationService = authorizationService;
            this._httpContextAccessor = httpContextAccessor;
            this._actionContextAccessor = actionContextAccessor;
        }

        public async Task GenerateLinks(AuthorDto authorDto)
        {
            var isAdmin = await IsAdmin();
            var url = ConstructUrlHelper();

            authorDto.Links.Add(new DataHATEOAS(url.Link("getAuthorById", new { id = authorDto.Id }), description: "self", method: "GET"));
            if(isAdmin)
            {
                authorDto.Links.Add(new DataHATEOAS(url.Link("updateAuthor", new { id = authorDto.Id }), description: "self", method: "PUT"));
                authorDto.Links.Add(new DataHATEOAS(url.Link("deleteAuthor", new { id = authorDto.Id }), description: "self", method: "DELETE"));
            }
        }

        private IUrlHelper ConstructUrlHelper()
        {
            var factory = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();

            return factory.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        private async Task<bool> IsAdmin()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var result = await _authorizationService.AuthorizeAsync(httpContext.User, "isAdmin");

            return result.Succeeded;
        }
    }
}
