using System.Net;
using System.Text.Json;
using FluentValidation;
using ProcurementApi.Application.Exceptions;

namespace ProcurementApi.Api.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                await WriteProblemAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ValidationException ex)
            {
                var detail = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage));
                await WriteProblemAsync(context, HttpStatusCode.BadRequest, detail);
            }
            catch (InvalidOperationException ex)
            {
                await WriteProblemAsync(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
                await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string detail)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var problem = new { status = (int)statusCode, title = statusCode.ToString(), detail };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}