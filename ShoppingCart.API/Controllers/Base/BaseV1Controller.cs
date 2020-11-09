using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;


namespace ShoppingCart.API.Controllers.Base
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BaseV1Controller : ControllerBase
    {
        protected virtual void LogInformation<LoggerType>(string message, params object[] args)
        {
            var log = HttpContext.RequestServices.GetService<ILogger<LoggerType>>();
            if (args.Length > 0)
            {
                log.LogInformation(message, args);
            }
            else
            {
                log.LogInformation(message);
            }
        }

        protected virtual void LogError<LoggerType>(Exception exception, string message = null, params object[] args)
        {
            var log = HttpContext.RequestServices.GetService<ILogger<BaseV1Controller>>();
            if (args.Length > 0)
            {
                log.LogError(exception, message ?? exception.Message, args);
            }
            else
            {
                log.LogError(exception, message ?? exception.Message);
            }
        }

        protected string IpAddress()
        {
            try
            {
                return Request.Headers.ContainsKey("X-Forwarded-For")
                    ? (string)Request.Headers["X-Forwarded-For"]
                    : HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            }
            catch (Exception e)
            {
                return "127.0.0.1";
            }
            
        }

    }
}
