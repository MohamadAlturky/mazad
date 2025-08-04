using System.Security.Cryptography;
using System.Text;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Services.Seeding
{
    public static class AdminUserSeeder
    {
        public static async Task SeedAdminUserAsync(IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<MazadDbContext>();

                await context.Database.MigrateAsync();

                if (!await context.Users.AnyAsync(u => u.UserType == UserType.Admin))
                {
                    var admin = new User
                    {
                        Name = "Admin",
                        PhoneNumber = "1234567890",
                        UserType = UserType.Admin,
                        Password = HashPassword("A@123456789"),
                        Email = "admin@sahel.com",
                    };

                    context.Users.Add(admin);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
