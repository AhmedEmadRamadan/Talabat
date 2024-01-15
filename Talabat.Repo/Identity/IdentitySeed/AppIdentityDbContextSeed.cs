using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repo.Identity.IdentitySeed
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "Ahmed Emad",
                    Email = "ao8856695@gmail.com",
                    PhoneNumber = "01146547343",
                    UserName = "ao8856695"
                };
                await userManager.CreateAsync(User, "P@ss0rd");
            }

        }
    }
}
