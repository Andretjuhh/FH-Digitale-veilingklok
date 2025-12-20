using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Attributes;

public class ValidateInput : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.HttpContext.Response.StatusCode = 400; // Bad Request
            context.Result = new Microsoft.AspNetCore.Mvc.JsonResult(context.ModelState);
        }
    }
}