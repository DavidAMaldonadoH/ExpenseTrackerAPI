using ExpenseTrackerAPI;
using ExpenseTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerAPI;

public static class SeedData
{
    public static void MigrateAndSeed(IServiceProvider _serviceProvider)
    {
        var context = _serviceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Groceries" },
                new Category { Name = "Leisure" },
                new Category { Name = "Electronics" },
                new Category { Name = "Utilities" },
                new Category { Name = "Clothing" },
                new Category { Name = "Health" },
                new Category { Name = "Others" }
            );

            context.SaveChanges();
        }

        if (!context.Users.Any())
        {
            context.Users.Add(
                new User
                {
                    Username = "test",
                    FirstName = "Test",
                    LastName = "User",
                    Password = PasswordHelper.HashPassword("pass1234")
                }
            );

            context.SaveChanges();
        }

    }
}