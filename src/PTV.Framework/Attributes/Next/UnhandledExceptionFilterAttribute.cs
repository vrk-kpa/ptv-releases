using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PTV.Framework.Exceptions;
using PTV.Framework.Exceptions.DataAccess;
using PTV.Framework.ServiceManager;

namespace PTV.Framework.Attributes.Next
{
    public class UnhandledExceptionModel
    {
        public string Message { get; set; }
    }

    [RegisterService(typeof(UnhandledExceptionFilterAttribute), RegisterType.Singleton)]
    public class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger logger;

        public UnhandledExceptionFilterAttribute(ILoggerFactory loggerFactory, IWebHostEnvironment env)
        {
            this.env = env;
            logger = loggerFactory.CreateLogger<UnhandledExceptionFilterAttribute>();
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogInformation("OnActionExecuting");
            if (context.Exception != null)
            {
                logger.LogError(context.Exception, "Server Exception");
                context.Result = BuildResult(context);
            }

            base.OnException(context);
        }

        private JsonResult BuildResult(ExceptionContext context)
        {
            var hostname = context?.HttpContext?.Request?.Host.Host?.ToLower() ?? "";
            var isDevOrTest = env.IsDevelopment() || hostname.Contains("dev.") || hostname.Contains("test.");

            if (context.Exception is PtvDbTooManyConnectionsException)
            {
                return BuildResult("Rejected: Too many connections to database. Try again later.",
                    StatusCodes.Status500InternalServerError);
            }

            if (context.Exception is OperationForbiddenException)
            {
                return BuildResult("Operation forbidden", StatusCodes.Status403Forbidden);
            }

            if (context.Exception is DuplicityCheckException duplicityCheckException)
            {
                logger.LogError("Duplicity error :", duplicityCheckException);
                return BuildProblemDetailsResult(new ProblemDetails
                {
                    Title = "Duplicity error",
                    Detail = "Ptv.Error.DuplicityError",
                    Instance = duplicityCheckException.ParamName
                }, StatusCodes.Status400BadRequest);
            }

            var msg = isDevOrTest ? context.Exception.FlattenWithInnerExceptions() : "Server error";
            return BuildResult(msg, StatusCodes.Status500InternalServerError);
        }

        private JsonResult BuildResult(string message, int statusCode)
        {
            var result = new JsonResult(new UnhandledExceptionModel {Message = message})
            {
                StatusCode = statusCode
            };
            return result;
        }

        private JsonResult BuildProblemDetailsResult(ProblemDetails details, int statusCode)
        {
            return new JsonResult(details)
            {
                StatusCode = statusCode
            };
        }
    }
}