using BlogVerse.Data;
using BlogVerse.Models;
using BlogVerse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList.Extensions;

namespace BlogVerse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            var data = new HomeViewModel();
            var setting = _context.Settings.ToList();
            data.Title = setting[0].Title;
            data.ShortDescription = setting[0].ShortDescription;
            data.ThumbnailUrl = setting[0].ThumbnailUrl;
            int pageNumber = (page ?? 1);
            int pageSize = 4;
            data.Posts = _context.Posts!.Include(x => x.ApplicationUser).OrderByDescending(x => x.CreatedDate).ToPagedList(pageNumber, pageSize);
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
