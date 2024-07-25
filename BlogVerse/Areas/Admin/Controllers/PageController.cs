using AspNetCoreHero.ToastNotification.Abstractions;
using BlogVerse.Data;
using BlogVerse.Models;
using BlogVerse.Utilities;
using BlogVerse.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
[Authorize(Roles = ApplicationRoles.AppAdmin)]
public class PageController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly INotyfService _notification;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PageController(ApplicationDbContext context, INotyfService notification,
        IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _notification = notification;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> About()
    {
        var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "about");
        var data = new PageViewModel()
        {
            Id = page.Id,
            Title = page.Title,
            ShortDescription = page.ShortDescription,
            Description = page.Description,
            ThumbnailUrl = page.ThumbnailUrl,
        };
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> About(PageViewModel model)
    {
        if (!ModelState.IsValid) { return View(); }
        var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "about");
        if (page == null)
        {
            _notification.Error("Page Not Found");
            return View();
        }
        page.Title = model.Title;
        page.ShortDescription = model.ShortDescription;
        page.Description = model.Description;        
        if (model.Thumbnail != null)
        {
            page.ThumbnailUrl = UploadFile(model.Thumbnail);
        }
        await _context.SaveChangesAsync();
        _notification.Success("Page updated successfully");
        return RedirectToAction("About", "Page", new {area="Admin"});
    }

    public async Task<IActionResult> Contact()
    {
        var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "contact");
        var data = new PageViewModel()
        {
            Id = page.Id,
            Title = page.Title,
            ShortDescription = page.ShortDescription,
            Description = page.Description,
            ThumbnailUrl = page.ThumbnailUrl,
        };
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Contact(PageViewModel model)
    {
        if (!ModelState.IsValid) { return View(); }
        var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "contact");
        if (page == null)
        {
            _notification.Error("Page Not Found");
            return View();
        }
        page.Title = model.Title;
        page.ShortDescription = model.ShortDescription;
        page.Description = model.Description;
        if (model.Thumbnail != null)
        {
            page.ThumbnailUrl = UploadFile(model.Thumbnail);
        }
        await _context.SaveChangesAsync();
        _notification.Success("Contact Page updated successfully");
        return RedirectToAction("Contact", "Page", new { area = "Admin" });
    }

    public async Task<IActionResult> Privacy()
    {
        var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "privacy");
        var data = new PageViewModel()
        {
            Id = page.Id,
            Title = page.Title,
            ShortDescription = page.ShortDescription,
            Description = page.Description,
            ThumbnailUrl = page.ThumbnailUrl,
        };
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Privacy(PageViewModel model)
    {
        if (!ModelState.IsValid) { return View(); }
        var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "privacy");
        if (page == null)
        {
            _notification.Error("Page Not Found");
            return View();
        }
        page.Title = model.Title;
        page.ShortDescription = model.ShortDescription;
        page.Description = model.Description;
        if (model.Thumbnail != null)
        {
            page.ThumbnailUrl = UploadFile(model.Thumbnail);
        }
        await _context.SaveChangesAsync();
        _notification.Success("Privacy updated successfully");
        return RedirectToAction("Privacy", "Page", new { area = "Admin" });
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
