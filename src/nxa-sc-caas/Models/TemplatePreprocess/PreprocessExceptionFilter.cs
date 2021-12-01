using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NXA.SC.Caas.Models
{
    public class PreprocessExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            if (exception.GetType().ToString().Contains("TemplatePreprocessException"))
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(exception.Message);
                context.ExceptionHandled = true;
            }
        }
    }
}
