using AspNetCoreHero.ToastNotification.Helpers;
using BlogVerse.Data;
using BlogVerse.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BlogVerse.Utilities
{
    public class DbInitilizer : IDbInitilizer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitilizer(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initilizer()
        {
            string[] roles = { ApplicationRoles.AppAdmin, ApplicationRoles.AppAuthor };
            foreach (var role in roles)
            {
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if(!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var admin = await _userManager.GetUsersInRoleAsync(ApplicationRoles.AppAdmin);
            if (!admin.Any())
            {
                var adminUser = new ApplicationUser()
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "superadmin@admin.com",
                    Email = "superadmin@admin.com"
                };
                var defaultPassword = "Admin@123";
                var result = await _userManager.CreateAsync(adminUser, defaultPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, ApplicationRoles.AppAdmin);
                }
            }
            var listOfPages = new List<Page>()
            {
                new Page()
                {
                    Title = "About Us",
                    Slug = "about"
                },
                new Page()
                {
                    Title = "Contect Us",
                    Slug = "contact"
                },
                new Page()
                {
                    Title = "Privacy Policy",
                    Slug = "privacy"
                },
            };
            await _context.Pages.AddRangeAsync(listOfPages);
            await _context.SaveChangesAsync();
        }
    }
}
