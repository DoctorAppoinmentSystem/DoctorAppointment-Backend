namespace DoctorAppointmentAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("[{Time}] {Method} {Path} from {IP}",
                DateTime.UtcNow, context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            await _next(context);

            _logger.LogInformation("[{Time}] Response: {Status}",
                DateTime.UtcNow, context.Response.StatusCode);
        }
    }

}
