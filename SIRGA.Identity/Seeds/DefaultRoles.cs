using Microsoft.AspNetCore.Identity;
using SIRGA.Identity.Shared.Enum;

namespace SIRGA.Identity.Seeds
{
	public class DefaultRoles
	{
		public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
		{
			
			await roleManager.CreateAsync(new IdentityRole(RolesEnum.Admin.ToString()));
			await roleManager.CreateAsync(new IdentityRole(RolesEnum.Estudiante.ToString()));
			await roleManager.CreateAsync(new IdentityRole(RolesEnum.Profesor.ToString()));

		}
	}
}
