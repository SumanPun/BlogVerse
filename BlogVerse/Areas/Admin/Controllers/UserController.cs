using AspNetCoreHero.ToastNotification.Abstractions;
using BlogVerse.Models;
using BlogVerse.ViewModels;
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

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
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
            return RedirectToAction("Index","User");
        }
    }
}
