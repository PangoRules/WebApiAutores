using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Utilities
{
    public class HATEOASFilterAttribute: ResultFilterAttribute
    {
        protected bool ShouldIncludeHATEOAS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;

            if(!IsSuccessfulResponse(result))
                return false;

            var headers = context.HttpContext.Request.Headers["includeHATEOAS"];

            if(headers.Count == 0)
                return false;

            var value = headers[0];

            if(!value.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        private bool IsSuccessfulResponse(ObjectResult result)
        {
            if(result == null || result.Value == null)
                return false;

            if(result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2"))
                return false;

            return true;
        }
    }
}
