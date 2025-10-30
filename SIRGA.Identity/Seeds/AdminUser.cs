using Microsoft.AspNetCore.Identity;
using SIRGA.Identity.Shared.Entities;
using SIRGA.Identity.Shared.Enum;

namespace SIRGA.Identity.Seeds
{
	public class AdminUser
	{
		public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			ApplicationUser user = new ApplicationUser
			{
				UserName = "The_real_Admin",
				Email = "AdminPro@SIGA.com",
				FirstName = "Admin",
				LastName = "Siga",
				Gender = 'F',
				DateOfBirth = DateOnly.Parse("1995-01-01"),
				Province = "Santo Domingo",
				Sector = "Centro",
				Address = "Calle Principal #123",
				IsActive = true,
				DateOfEntry = DateOnly.Parse("2020-01-01"),
				CreatedAt = DateOnly.FromDateTime(DateTime.Now),
				LastLogin = DateTimeOffset.Now,
				EmailConfirmed = true,
				PhoneNumberConfirmed = true,
                MustCompleteProfile = false

            };

			if (await userManager.FindByEmailAsync(user.Email) == null)
			{
				// creando la cuenta de admin
				var result = await userManager.CreateAsync(user, "Admin#123");

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"❌ Error creando admin: {error.Code} - {error.Description}");
                    }
                }
                else
                {
                    await userManager.AddToRoleAsync(user, RolesEnum.Admin.ToString());
                }
            }
		}
	}
}
