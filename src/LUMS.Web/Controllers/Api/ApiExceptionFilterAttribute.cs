using LUMS.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LUMS.Web.Controllers.Api;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var (status, title) = context.Exception switch
        {
            DomainException => (StatusCodes.Status409Conflict, "Vi phạm quy tắc nghiệp vụ"),
            NotFoundException => (StatusCodes.Status404NotFound, "Không tìm thấy dữ liệu"),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Không có quyền thực hiện"),
            _ => (0, string.Empty)
        };

        if (status == 0)
        {
            return;
        }

        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = context.Exception.Message
        })
        {
            StatusCode = status
        };
        context.ExceptionHandled = true;
    }
}
