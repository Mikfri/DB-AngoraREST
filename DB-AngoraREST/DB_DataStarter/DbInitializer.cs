using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;

namespace DB_AngoraREST.DB_DataStarter
{
    public static class DbInitializer
    {
        public static void Initialize(DB_AngoraContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.Users.Any())
            {
                return;   // DB has already been seeded
            }

            var mockUsers = MockUsers.GetMockUsers();
            foreach (var user in mockUsers)
            {
                userManager.CreateAsync(user, user.Password).Wait();
            }

            var mockRabbits = MockRabbits.GetMockRabbits();
            foreach (var rabbit in mockRabbits)
            {
                context.Rabbits.Add(rabbit);
            }

            context.SaveChanges();
        }
    }
}
