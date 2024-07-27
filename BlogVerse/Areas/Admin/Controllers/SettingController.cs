using AspNetCoreHero.ToastNotification.Abstractions;
using BlogVerse.Data;
using BlogVerse.Models;
using BlogVerse.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BlogVerse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingController(ApplicationDbContext context, INotyfService notification,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _notification = notification;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var settings = _context.Settings.ToList();
            if (settings.Count > 0)
            {
                var data = new SettingViewModel()
                {
                    Id = settings[0].Id,
                    SiteName = settings[0].SiteName,
                    Title = settings[0].Title,
                    ShortDescription = settings[0].ShortDescription,
                    ThumbnailUrl = settings[0].ThumbnailUrl,
                    FacebookUrl = settings[0].FacebookUrl,
                    LinkdenUrl = settings[0].LinkdenUrl,
                    GithubUrl = settings[0].GithubUrl
                };
                return View(settings);
            }
            var setting = new Setting()
            {
                SiteName = "Demo Name"
            };
            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();
            var createdSettings = _context.Settings.ToList();
            var result = new SettingViewModel()
            {
                Id = createdSettings[0].Id,
                SiteName = createdSettings[0].SiteName,
                Title = createdSettings[0].Title,
                ShortDescription = createdSettings[0].ShortDescription,
                ThumbnailUrl = createdSettings[0].ThumbnailUrl,
                FacebookUrl = createdSettings[0].FacebookUrl,
                LinkdenUrl = createdSettings[0].LinkdenUrl,
                GithubUrl = createdSettings[0].GithubUrl
            };
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Index(SettingViewModel model)
        {
            if (!ModelState.IsValid) { return View(); }
            var setting = await _context.Settings!.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (setting == null)
            {
                _notification.Error("Something went wrong Site Setting");
                return View(model);
            }
            setting.SiteName = model.SiteName;
            setting.Title = model.Title;
            setting.ShortDescription = model.ShortDescription;
            setting.FacebookUrl = model.FacebookUrl;
            setting.LinkdenUrl = model.LinkdenUrl;
            setting.GithubUrl = model.GithubUrl;
            if (model.Thumbnail != null)
            {
                setting.ThumbnailUrl = UploadFile(model.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Setting updated successfully");
            return RedirectToAction("Index", "Setting", new { area = "Admin" });
        }

        private string UploadFile(IFormFile file)
        {
            string fileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
            fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, fileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return fileName;
        }
    }
}
