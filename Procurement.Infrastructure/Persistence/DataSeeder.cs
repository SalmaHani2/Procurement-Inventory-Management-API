using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using ProcurementApi.Domain.Entities;
using ProcurementApi.Infrastructure.Identity;

namespace ProcurementApi.Infrastructure.Persistence
{
    /// <summary>Seeds roles, demo users, and a starter catalog so the API is explorable immediately after first run.</summary>
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<ProcurementDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await EnsureDemoUserAsync(userManager, "admin@procurement.demo", "Demo Admin", Roles.Admin, "Admin#12345");
            await EnsureDemoUserAsync(userManager, "manager@procurement.demo", "Demo Manager", Roles.Manager, "Manager#12345");
            await EnsureDemoUserAsync(userManager, "clerk@procurement.demo", "Demo Clerk", Roles.Clerk, "Clerk#12345");

            if (!dbContext.Suppliers.Any())
            {
                var supplier = new Supplier
                {
                    Name = "Acme Building Materials",
                    ContactEmail = "sales@acme-materials.example",
                    Phone = "+91 98765 43210",
                    IsActive = true,
                    CreatedBy = "seed",
                    CreatedOn = DateTime.UtcNow
                };

                dbContext.Suppliers.Add(supplier);
                await dbContext.SaveChangesAsync();

                dbContext.Products.AddRange(
                    new Product
                    {
                        Sku = "CEM-001",
                        Name = "Portland Cement (50kg bag)",
                        UnitPrice = 420m,
                        StockOnHand = 120,
                        ReorderThreshold = 50,
                        SupplierId = supplier.Id,
                        CreatedBy = "seed",
                        CreatedOn = DateTime.UtcNow
                    },
                    new Product
                    {
                        Sku = "REB-012",
                        Name = "Steel Rebar 12mm (per tonne)",
                        UnitPrice = 58000m,
                        StockOnHand = 8,
                        ReorderThreshold = 10,
                        SupplierId = supplier.Id,
                        CreatedBy = "seed",
                        CreatedOn = DateTime.UtcNow
                    });

                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task EnsureDemoUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string displayName,
            string role,
            string password)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
            {
                return;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = displayName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}