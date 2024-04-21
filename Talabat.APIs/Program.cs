
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Infrastructure;
using Talabat.Infrastructure.Data;

namespace Talabat.APIs
{
	public class Program
	{
		//Entry Point
		public static async Task Main(string[] args)
		{

			var webApplicationbuilder = WebApplication.CreateBuilder(args);

			#region Configure Services
			// Add services to DI the container.

			webApplicationbuilder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			webApplicationbuilder.Services.AddEndpointsApiExplorer();
			webApplicationbuilder.Services.AddSwaggerGen();

			webApplicationbuilder.Services.AddDbContext<StoreContext>(options =>
			{
				options.UseSqlServer(webApplicationbuilder.Configuration.GetConnectionString("DefaultConnection"));

			});
			///webApplicationbuilder.Services.AddScoped<IGenericRepository<Product>,GenericRepository<Product>>();
			///webApplicationbuilder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();
			///webApplicationbuilder.Services.AddScoped<IGenericRepository<ProductCategory>, GenericRepository<ProductCategory>>();

			webApplicationbuilder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
			//webApplicationbuilder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
			webApplicationbuilder.Services.AddAutoMapper(typeof(MappingProfiles));

			webApplicationbuilder.Services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
					.SelectMany(P => P.Value.Errors)
					.Select(E => E.ErrorMessage)
					.ToArray();
					var response = new ApiValidationErrorResponse()
					{
						Errors = errors
					};

					return new BadRequestObjectResult(response);
				};
				
			});



			#endregion

			var app = webApplicationbuilder.Build();

			using var Scope = app.Services.CreateScope();
			 var Services=Scope.ServiceProvider;
			var _dbContext=Services.GetRequiredService<StoreContext>();
			//Ask CLR for creating object from DbContext Explicitly

			//Ask CLR  for creating object to log if there is a problem in updating
			var loggerFactory=Services.GetRequiredService<ILoggerFactory>();
			var logger = loggerFactory.CreateLogger<Program>();

			try
			{
				await _dbContext.Database.MigrateAsync(); //Update-Database

				await StoreContextSeed.SeedAsync(_dbContext);
			}
			catch (Exception ex) 
			{
                Console.WriteLine(ex);

				logger.LogError(ex, "An Error Has been occured while applying Migration");

            }


			#region Configure Kestrel MiddleWares


			// Configure the HTTP request pipeline.
			//app.UseMiddleware<ExceptionMiddleware>();

			app.Use(async (httpcontext, _next) =>
			{
				try
				{
					//take an action with request
					await _next.Invoke(httpcontext); //Go To Next Middleware

					//take action with response
				}
				catch (Exception ex)
				{

					logger.LogError(ex.Message); //Development

					//log exception In Database or file in production Env

					httpcontext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					httpcontext.Response.ContentType = "application/json";

					var response = app.Environment.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
						: new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

					var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
					var json = JsonSerializer.Serialize(response, options);

					await httpcontext.Response.WriteAsync(json);

				}
			});
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseStatusCodePagesWithReExecute("/Errors/{0}");

			app.UseHttpsRedirection();


			app.UseStaticFiles();

			app.MapControllers(); 
			#endregion




			app.Run();
		}
	}
}
