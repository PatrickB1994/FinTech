namespace api.Helpers
{
    public class HttpExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpException ex)
            {
                // Handle custom HttpStatusException and set the response code
                context.Response.StatusCode = (int)ex.StatusCode;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    error = ex.Message,
                    statusCode = ex.StatusCode
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception e)
            {
                // Handle other exceptions (if needed)
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = e.Message, statusCode = 500 });
            }
        }
    }
}