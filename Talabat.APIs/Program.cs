
using Microsoft.EntityFrameworkCore;
using Talabat.Infrastructure.Data;

namespace Talabat.APIs
{
	public class Program
	{
		//Entry Point
		public static void Main(string[] args)
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
			#endregion

			var app = webApplicationbuilder.Build();

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
