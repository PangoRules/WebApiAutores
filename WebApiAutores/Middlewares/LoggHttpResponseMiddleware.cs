namespace WebApiAutores.Middlewares
{
    public static class LoggHttpResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggHttpResponse(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggHttpResponseMiddleware>();
        }
    }

    public class LoggHttpResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggHttpResponseMiddleware(RequestDelegate next, ILogger<LoggHttpResponseMiddleware> logger)
        {
            this._next = next;
            Logger = logger;
        }

        public ILogger<LoggHttpResponseMiddleware> Logger { get; }

        //Invoke o InvokeAsync.
        public async Task InvokeAsync(HttpContext context)
        {
            using(var ms = new MemoryStream())
            {
                var responseBody = context.Response.Body;
                context.Response.Body = ms;

                await _next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(responseBody);
                context.Response.Body = responseBody;

                Logger.LogInformation(response);
            }
        }
    }
}
