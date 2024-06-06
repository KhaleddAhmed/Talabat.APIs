using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Infrastructure._Identity
{
	public static class ApplicationIdentityDataSeed
	{
		public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager)
		{
			if (!userManager.Users.Any())
			{
				var user = new ApplicationUser()
				{
					DisplayName = "Khaled Ahmed",
					Email = "Khaled.Fk@gmail.com",
					UserName = "Khaled.Ahmed",
					PhoneNumber = "01092988291"
				}; 

				await userManager.CreateAsync(user,"P@ssw0rd");
			}
		}
	}
}
