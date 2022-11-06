using System.Diagnostics;
using Funda.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Funda.Web.Api.Filters;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not BusinessLogicException exception)
            return;

        var status = exception.Code switch
        {
            ErrorCode.NotFound => StatusCodes.Status404NotFound,
            ErrorCode.InvalidOperation or ErrorCode.Unknown => StatusCodes.Status400BadRequest,
            _ => throw new NotImplementedException(),
        };

        var responseObj = new
        {
            TraceId = Activity.Current?.Id ?? context.HttpContext?.TraceIdentifier,
            Status = status,
            Message = exception.Message,
        };

        context.Result = new JsonResult(responseObj) { StatusCode = status };

        context.ExceptionHandled = true;
    }
}
