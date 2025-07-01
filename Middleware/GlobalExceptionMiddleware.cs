using System.Net;
using System.Text.Json;
using TodoApi.Models;

namespace TodoApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite : {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ErrorResponse
            {
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case ArgumentNullException:
                    response.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    response.Title = "Paramètre manquant";
                    response.Status = (int)HttpStatusCode.BadRequest;
                    response.Detail = exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case ArgumentException:
                    response.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    response.Title = "Paramètre invalide";
                    response.Status = (int)HttpStatusCode.BadRequest;
                    response.Detail = exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case KeyNotFoundException:
                    response.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    response.Title = "Ressource non trouvée";
                    response.Status = (int)HttpStatusCode.NotFound;
                    response.Detail = exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    response.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                    response.Title = "Erreur interne du serveur";
                    response.Status = (int)HttpStatusCode.InternalServerError;
                    response.Detail = "Une erreur inattendue s'est produite. Veuillez réessayer plus tard.";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
} 