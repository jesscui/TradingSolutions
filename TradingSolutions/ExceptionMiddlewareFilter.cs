using System.Net;
using System.Text.Json;
using TradingSolutions.Application.Models;

namespace TradingSolutions
{
    public class ExceptionMiddlewareFilter
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddlewareFilter(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new ExecutionResult
            {
                IsError = true
            };
            switch (exception)
            {
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.ErrorDetails = new string[] { "Internal server error" };
                    break;
            }
            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
