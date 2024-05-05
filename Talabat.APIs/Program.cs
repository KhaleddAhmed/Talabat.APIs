
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Text.Json;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Infrastructure;
using Talabat.Infrastructure._Identity;
using Talabat.Infrastructure.Data;
using Talabat.Service.AuthService;

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

			webApplicationbuilder.Services.AddSwaggerServices();

			webApplicationbuilder.Services.AddDbContext<StoreContext>(options =>
			{
				options.UseSqlServer(webApplicationbuilder.Configuration.GetConnectionString("DefaultConnection"));

			});

			webApplicationbuilder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
			{
				options.UseSqlServer(webApplicationbuilder.Configuration.GetConnectionString("IdentityConnection"));
			});
			//ApplicationServicesExtension.AddApplicationServices(webApplicationbuilder.Services);
			webApplicationbuilder.Services.AddApplicationServices();

			webApplicationbuilder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvidee) =>
			{
				var connection = webApplicationbuilder.Configuration.GetConnectionString("Redis");
				return ConnectionMultiplexer.Connect(connection);
			});

			webApplicationbuilder.Services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			webApplicationbuilder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));

			webApplicationbuilder.Services.AddAuthServices(webApplicationbuilder.Configuration);

			
			#endregion

			var app = webApplicationbuilder.Build();

			#region Updata Database and DataSeeding
			using var Scope = app.Services.CreateScope();
			var Services = Scope.ServiceProvider;
			var _dbContext = Services.GetRequiredService<StoreContext>();
			var _identitydbContext = Services.GetRequiredService<ApplicationIdentityDbContext>();
			//Ask CLR for creating object from DbContext Explicitly

			//Ask CLR  for creating object to log if there is a problem in updating
			var loggerFactory = Services.GetRequiredService<ILoggerFactory>();
			var logger = loggerFactory.CreateLogger<Program>();

			try
			{
				await _dbContext.Database.MigrateAsync(); //Update-Database
				await StoreContextSeed.SeedAsync(_dbContext);

				await _identitydbContext.Database.MigrateAsync();//Update-Database
				var _userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();
				await ApplicationIdentityDataSeed.SeedUserAsync(_userManager);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);

				logger.LogError(ex, "An Error Has been occured while applying Migration");

			} 
			#endregion


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
				app.UseSwaggerMiddlewares();

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
