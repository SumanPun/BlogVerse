using AspNetCoreHero.ToastNotification.Abstractions;
using BlogVerse.Data;
using BlogVerse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogVerse.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;

        public BlogController(ApplicationDbContext context, INotyfService notification)
        {
            _context = context;
            _notification = notification;
        }
        [HttpGet("[controller]/{slug}")]
        public IActionResult Post(string slug)
        {
            if(slug == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            var post = _context.Posts!.Include(x => x.ApplicationUser).FirstOrDefault(x => x.Slug == slug);
            if(post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            var data = new BlogPostViewModel()
            {
                Id = post.Id,
                Title = post.Title,
                AuthorName = post.ApplicationUser!.FirstName + " " + post.ApplicationUser.LastName,
                ThumbnailUrl = post.ThumbnailUrl,
                ShortDescription = post.ShortDescription,
                Description = post.Description
            };
            return View(data);
        }
    }
}
