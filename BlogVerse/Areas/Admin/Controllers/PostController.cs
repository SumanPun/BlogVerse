using AspNetCoreHero.ToastNotification.Abstractions;
using BlogVerse.Data;
using BlogVerse.Models;
using BlogVerse.Utilities;
using BlogVerse.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace BlogVerse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ApplicationDbContext context, INotyfService notification,
            IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notification = notification;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var listOfPosts = new List<Post>();
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] == ApplicationRoles.AppAdmin)
            {
                listOfPosts = await _context.Posts!.Include(x => x.ApplicationUser).ToListAsync();
            }
            else
            {
                listOfPosts = await _context.Posts!.Include(_ => _.ApplicationUser).Where(x => x.ApplicationUser!.Id == loggedInUser!.Id).ToListAsync();
            }

            var data = listOfPosts.Select(x => new PostViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                CreatedDate = x.CreatedDate,
                ThumbnailUrl = x.ThumbnailUrl,
                AuthorName = x.ApplicationUser!.FirstName + " " + x.ApplicationUser!.LastName
            }).ToList();
            int pageNumber = (page ?? 1);
            int pageSize = 4;
            return View(data.OrderByDescending(x => x.CreatedDate).ToPagedList(pageNumber,pageSize));
        }

        public IActionResult Create()
        {
            return View(new CreatePostViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (!ModelState.IsValid) { return View(); }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

            var post = new Post()
            {
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                Description = model.Description,
                ApplicationUserId = loggedInUser!.Id
            };
            if (model.Title != null)
            {
                string slug = model.Title.Trim();
                slug = slug.Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }
            if (model.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadFile(model.Thumbnail);
            }
            await _context.Posts!.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Posts Created Successfully!!");
            return RedirectToAction("Index");
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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] == ApplicationRoles.AppAdmin || loggedInUser?.Id == post?.ApplicationUserId)
            {
                _context.Posts!.Remove(post!);
                await _context.SaveChangesAsync();
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
            {
                _notification.Error("Post Not Found");
                return View();
            }
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] != ApplicationRoles.AppAdmin && loggedInUser?.Id != post?.ApplicationUserId)
            {
                _notification.Error("You are not authorized");
                return RedirectToAction("Index");
            }
            var data = new CreatePostViewModel()
            {
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Description = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,
                CreatedDate = post.CreatedDate
            };
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }
            var post = await _context.Posts!.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            post.Title = model.Title;
            post.ShortDescription = model.ShortDescription;
            post.Description = model.Description;
            if (model.Title != null)
            {
                string slug = model.Title.Trim();
                slug = slug.Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }
            if (model.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadFile(model.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Post edit successfully");
            return RedirectToAction("Index","Post", new {area="Admin"});
        }
    }
}