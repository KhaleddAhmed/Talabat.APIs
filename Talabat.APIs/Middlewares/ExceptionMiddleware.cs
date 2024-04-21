
using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
	//By CONVENTION
	public class ExceptionMiddleware
	{
		//private readonly RequestDelegate _next;
		private readonly Microsoft.Extensions.Logging.ILogger<ExceptionMiddleware> _logger;
		private readonly IWebHostEnvironment _env;

		public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger,IWebHostEnvironment env)
        {
			//_next = next;
			_logger = logger;
			_env = env;
		}

        public async Task InvokeAsync(HttpContext context,RequestDelegate _next)
		{
			try
			{
				//take an action with request
				await _next.Invoke(context); //Go To Next Middleware

				//take action with response
			}
			catch (Exception ex)
			{

				_logger.LogError(ex.Message); //Development

				//log exception In Database or file in production Env

				context.Response.StatusCode =(int) HttpStatusCode.InternalServerError;
				context.Response.ContentType = "application/json";

				var response = _env.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
					:new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

				var options=new JsonSerializerOptions() { PropertyNamingPolicy=JsonNamingPolicy.CamelCase };
				var json=JsonSerializer.Serialize(response,options);

				await context.Response.WriteAsync(json);

			}
		}
	}
}
