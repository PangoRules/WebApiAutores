using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutores.DTOs;

namespace WebApiAutores.Utilities
{
    public class HATEOASAuthorFilterAttribute: HATEOASFilterAttribute
    {
        private readonly Services.LinkGenerator _linkGenerator;

        public HATEOASAuthorFilterAttribute(Services.LinkGenerator linkGenerator)
        {
            this._linkGenerator = linkGenerator;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var shouldInclude = ShouldIncludeHATEOAS(context);

            if(!shouldInclude)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;

            var authorDto = result.Value as AuthorDto;
            if(authorDto == null)
            {
                var authorsDto = result.Value as List<AuthorDto> ?? throw new ArgumentException("Instance of AuthorDto or List<AuthorDto> expecting.");

                authorsDto.ForEach(async author => await _linkGenerator.GenerateLinks(author));

                result.Value = authorsDto;
            }
            else
                await _linkGenerator.GenerateLinks(authorDto);

            await next();
        }
    }
}
