using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace MolTol.Data;

public static class IdentitySeed
{
    private static readonly string[] Roles = { "User", "Admin" };

    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();

        var serviceProvider = scope.ServiceProvider;

        var roleManager = serviceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = serviceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var configuration = serviceProvider
            .GetRequiredService<IConfiguration>();

        foreach (var role in Roles)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(role));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create the '{role}' role: {errors}");
            }
        }

        var adminEmail = configuration["AdminAccount:Email"];
        var adminPassword = configuration["AdminAccount:Password"];
        var adminFullName = configuration["AdminAccount:FullName"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword) ||
            string.IsNullOrWhiteSpace(adminFullName))
        {
            throw new InvalidOperationException(
                "Admin account configuration is missing. Configure AdminAccount:Email, AdminAccount:Password, and AdminAccount:FullName using User Secrets.");
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = adminFullName,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(
                adminUser,
                adminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    createResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create the Admin account: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                adminUser,
                "Admin");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    roleResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to assign the Admin role: {errors}");
            }
        }
    }
}
