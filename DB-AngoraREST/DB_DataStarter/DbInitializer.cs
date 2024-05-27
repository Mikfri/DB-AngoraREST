using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DB_AngoraREST.DB_DataStarter
{
    public class DbInitializer
    {
        public static void Initialize(DB_AngoraContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Hvis databasen allerede indeholder brugere, skal vi ikke gøre noget mere.
            if (context.Users.Any())
            {
                return;
            }

            // Opret roller
            var mockRoles = MockRoles.GetMockRoles();
            foreach (var role in mockRoles)
            {
                if (!roleManager.RoleExistsAsync(role.Name).Result)
                {
                    var result = roleManager.CreateAsync(role).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role.Name}");
                    }
                }
            }

            // Tilføj RoleClaims til roller
            var roleClaims = RoleClaims.Get_AspNetRoleClaims();
            foreach (var roleClaim in roleClaims)
            {
                var role = roleManager.FindByIdAsync(roleClaim.RoleId).Result;
                if (role != null)
                {
                    var claim = new Claim(roleClaim.ClaimType, roleClaim.ClaimValue);
                    var hasClaim = roleManager.GetClaimsAsync(role).Result.Any(c => c.Type == claim.Type && c.Value == claim.Value);
                    if (!hasClaim)
                    {
                        var result = roleManager.AddClaimAsync(role, claim).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Failed to add claim {claim.Type} to role {role.Name}");
                        }
                    }
                }
            }

            //// Brug MockDataInitializer til at tilføje mock brugere og deres roller
            //var mockDataInitializer = new MockDataInitializer(context, userManager);    // TODO: Test om dette virker fremfor nedestående //'kode'
            //mockDataInitializer.Initialize();
                       

            // Hent mock brugere og deres roller
            var mockUsersWithRoles = MockUsers.GetMockUsersWithRoles();

            foreach (var mockUserWithRole in mockUsersWithRoles)
            {
                // Opret brugeren
                var user = mockUserWithRole.User;
                var result = userManager.CreateAsync(user, user.Password).Result;

                if (result.Succeeded)
                {
                    // Tildel roller til brugeren
                    foreach (var role in mockUserWithRole.Roles)
                    {
                        if (roleManager.RoleExistsAsync(role).Result)
                        {
                            userManager.AddToRoleAsync(user, role).Wait();
                        }
                    }

                    // Tildel unikke UserClaims til brugeren
                    var userClaims = MockUserClaims.GetMockUserClaimsForUser(user);
                    userManager.AddClaimsAsync(user, userClaims).Wait();
                }
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
