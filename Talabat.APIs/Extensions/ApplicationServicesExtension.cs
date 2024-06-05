using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Infrastructure;
using Talabat.Infrastructure.BasketRepository;
using Talabat.Infrastructure.GenericRepository;
using Talabat.Service;
using Talabat.Service.OrderService;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
		{
			//Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			//webApplicationbuilder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
		Services.AddAutoMapper(typeof(MappingProfiles));

			Services.Configure<ApiBehaviorOptions>(options =>
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

			Services.AddScoped(typeof(IBasketRepository),typeof(BasketRepository));


			Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

			Services.AddScoped(typeof(IOrderService), typeof(OrderService));

			Services.AddScoped(typeof(IProductService),typeof(ProductService));

			return Services;

		}

		public static IServiceCollection  AddAuthServices(this IServiceCollection Services,IConfiguration configuration)
		{
			Services.AddAuthentication(/*JwtBearerDefaults.AuthenticationScheme*/ options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateIssuer = true,
			ValidIssuer = configuration["JWT:ValidIssuer"],
			ValidateAudience = true,
			ValidAudience = configuration["JWT:ValidAudience"],
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:AuthKey"])),
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero,

		};
	});
			return Services;
		}
	}
}
