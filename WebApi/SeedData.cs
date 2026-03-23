using Entities.Concrete.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi
{
    public static class SeedData
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            // Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Admin user (appsettings-dən oxu)
            var adminEmail = configuration["AdminSeed:Email"];
            var adminPassword = configuration["AdminSeed:Password"];

            // təhlükəsizlik: boşdursa seed etmə
            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                return;

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin"
                };

                var created = await userManager.CreateAsync(admin, adminPassword);
                if (created.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                if (!await userManager.IsInRoleAsync(admin, "Admin"))
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}