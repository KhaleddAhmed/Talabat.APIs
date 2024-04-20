
using Microsoft.EntityFrameworkCore;
using Talabat.APIs.Helpers;
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



			#endregion

			var app = webApplicationbuilder.Build();

			using var Scope = app.Services.CreateScope();
			 var Services=Scope.ServiceProvider;
			var _dbContext=Services.GetRequiredService<StoreContext>();
			//Ask CLR for creating object from DbContext Explicitly

			//Ask CLR  for creating object to log if there is a problem in updating
			var loggerFactory=Services.GetRequiredService<ILoggerFactory>();
			try
			{
				await _dbContext.Database.MigrateAsync(); //Update-Database

				await StoreContextSeed.SeedAsync(_dbContext);
			}
			catch (Exception ex) 
			{
                Console.WriteLine(ex);

				var logger=loggerFactory.CreateLogger<Program>();
				logger.LogError(ex, "An Error Has been occured while applying Migration");

            }
			

			#region Configure Kestrel MiddleWares
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();




			app.MapControllers(); 
			#endregion




			app.Run();
		}
	}
}
