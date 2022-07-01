using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Tweet.Core.Exceptions;

namespace Tweet.Core.Attributes
{
    public class CustomExceptionAttibute : Attribute, IExceptionFilter
    {

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is CustomException)
            {
                context.HttpContext.Response.StatusCode = (context.Exception as CustomException).StatusCode;
            }
            else
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            context.Result = new ObjectResult(context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }
}
