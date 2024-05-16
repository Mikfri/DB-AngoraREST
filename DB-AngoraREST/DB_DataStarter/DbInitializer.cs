using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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

            // Create roles and add claims to them
            var mockRoles = MockRoles.GetMockRoles();
            foreach (var role in mockRoles)
            {
                if (!roleManager.RoleExistsAsync(role.Name).Result)
                {
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;

                    if (roleResult.Succeeded)
                    {
                        // Get the claims for this role
                        var roleClaims = MockRoleClaims.GetMockRoleClaims().Where(rc => rc.RoleId == role.Id);

                        foreach (var claim in roleClaims)
                        {
                            roleManager.AddClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue)).Wait();
                        }
                    }
                }
            }

            var mockUsersWithRoles = MockUsers.GetMockUsersWithRoles();
            foreach (var mockUserWithRole in mockUsersWithRoles)
            {
                userManager.CreateAsync(mockUserWithRole.User, mockUserWithRole.User.Password).Wait();
                userManager.AddToRoleAsync(mockUserWithRole.User, mockUserWithRole.Role).Wait();
            }

            var mockRabbits = MockRabbits.GetMockRabbits();
            foreach (var rabbit in mockRabbits)
            {
                // Set the User property of the Rabbit object to the corresponding User
                rabbit.User = context.Users.FirstOrDefault(u => u.Id == rabbit.OwnerId);
                context.Rabbits.Add(rabbit);
            }

            context.SaveChanges();
        }
    }
}
