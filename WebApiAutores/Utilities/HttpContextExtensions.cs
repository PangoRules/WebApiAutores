

using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task InsertParametersPaginationInHeaders<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if(httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            double amount = await queryable.CountAsync();
            httpContext.Response.Headers.Add("totalAmountRecords", amount.ToString());
        }
    }
}
