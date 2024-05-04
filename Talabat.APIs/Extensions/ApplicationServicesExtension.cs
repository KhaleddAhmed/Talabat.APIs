using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Repositories.Contract;
using Talabat.Infrastructure.BasketRepository;
using Talabat.Infrastructure.GenericRepository;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
		{
			Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
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

			

			return Services;

		}
	}
}
