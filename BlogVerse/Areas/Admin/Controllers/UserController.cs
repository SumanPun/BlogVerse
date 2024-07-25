using AspNetCoreHero.ToastNotification.Abstractions;
using BlogVerse.Models;
using BlogVerse.Utilities;
using BlogVerse.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogVerse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            INotyfService notification)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notification = notification;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var data = users.Select(x => new UserViewModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            }).ToList();

            foreach(var user in data)
            {
                var singleUser = await _userManager.FindByIdAsync(user.Id);
                var role = await _userManager.GetRolesAsync(singleUser);
                user.Role = role.FirstOrDefault();
            }
            return View(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if(existingUser == null)
            {
                _notification.Error("User doesnot exist");
                return View();
            }
            var data = new ResetPasswordViewModel()
            {
                Id = existingUser.Id,
                UserName = existingUser.UserName
            };
            return View(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(!ModelState.IsValid) { return View(model); }
            var existingUser = await _userManager.FindByIdAsync(model.Id);
            if (existingUser == null)
            {
                _notification.Error("User doesnot exist");
                return View(model);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var result = await _userManager.ResetPasswordAsync(existingUser, token, model.NewPassword);
            if (result.Succeeded)
            {
                _notification.Success("Password Changed Successfully");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Register()
        {
           
            return View(new RegisterViewModel());
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(!ModelState.IsValid) { return View(model); }
            
            var checkUserName = await _userManager.FindByNameAsync(model.UserName);
            if(checkUserName != null)
            {
                _notification.Error("UserName already Exist!!");
                return View(model);
            }
            var checkEmail = await _userManager.FindByEmailAsync(model.Email);
            if (checkEmail != null)
            {
                _notification.Error("Email already Exist!!");
                return View(model);
            }

            var applicationUser = new ApplicationUser()
            {
                FirstName = model.UserName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(applicationUser, model.Password);
            if (result.Succeeded)
            {
                if (model.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(applicationUser, ApplicationRoles.AppAdmin);
                }
                else
                {
                    await _userManager.AddToRoleAsync(applicationUser, ApplicationRoles.AppAuthor);
                }
                _notification.Success("User Register Successfully");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            return View(model);
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            if(!HttpContext.User.Identity!.IsAuthenticated)
            {
                return View(new LoginViewModel());
            }
            return RedirectToAction("Index","Post", new {area="Admin"});
            
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(!ModelState.IsValid) { return View(model); }
                
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == model.Username);
            if(existingUser == null)
            {
                _notification.Error("username doesnot exist");
                return View(model);
            }
            var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, model.Password);
            if (!verifyPassword)
            {
                _notification.Error("Password does not match");
            }
            await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, true);
            _notification.Success("Login Successfull");
            return RedirectToAction("Index","Post", new { area = "Admin" });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _notification.Success("Logout Successfully");
            return RedirectToAction("Index", "Home", new {area=""});
        }

        [HttpGet("AccessDenied")]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return RedirectToAction();
        }
    }
}
