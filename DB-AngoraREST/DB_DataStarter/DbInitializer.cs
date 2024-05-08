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
                // Set the User property of the Rabbit object to the corresponding User
                rabbit.User = context.Users.FirstOrDefault(u => u.Id == rabbit.OwnerId);
                context.Rabbits.Add(rabbit);
            }

            // Create roles
            var mockRoles = MockRoles.GetMockRoles();
            foreach (var role in mockRoles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }

            context.SaveChanges();
        }


    }
}
