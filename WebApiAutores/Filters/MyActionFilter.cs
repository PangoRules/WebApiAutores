using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filters
{
    public class MyActionFilter : IActionFilter
    {
        private readonly ILogger<MyActionFilter> _logger;

        public MyActionFilter(ILogger<MyActionFilter> logger)
        {
            this._logger = logger;
        }

        //Executes before executing an action
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("Before executing action.");
        }

        //Executes when an action is being executed
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("After executing action.");
        }

        
    }
}
