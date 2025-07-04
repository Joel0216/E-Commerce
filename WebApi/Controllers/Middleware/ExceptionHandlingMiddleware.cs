using Application.Exceptions;
using System.Net;

namespace WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                
                // Log del error para debugging
                Console.WriteLine($"❌ Error 500: {ex.Message}");
                Console.WriteLine($"📍 Stack Trace: {ex.StackTrace}");
                
                await context.Response.WriteAsJsonAsync(new { 
                    error = "Error interno del servidor.",
                    details = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }
    }
}
