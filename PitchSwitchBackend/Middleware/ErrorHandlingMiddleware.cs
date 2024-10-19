namespace PitchSwitchBackend.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
                _logger.LogError($"Exception thrown while processing the request:\nMessage: {ex.Message}\nException: \n{ex}");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var message = "An internal server error occurred while processing the request";

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
