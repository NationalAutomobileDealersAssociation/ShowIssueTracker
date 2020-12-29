using Microsoft.Extensions.Logging;
using System;

namespace ShowIssueTracker.Authorization
{
    public class GlobalExceptionLoggingMiddleWare : IGlobalExceptionLoggingMiddleWare
    {
       // private readonly RequestDelegate _next;
        private readonly ILogger _logger;

       public GlobalExceptionLoggingMiddleWare(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GlobalExceptionLoggingMiddleWare>();
           // _next = next;
        }

        public void Invoke(Exception ex, string message)
        {
            try
            {
                
                _logger.LogError(ex, message);
                _logger.LogError("\n **********************************************************");
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, message);
                _logger.LogError("\n **********************************************************");
                return;
            }

        }

    }
}
