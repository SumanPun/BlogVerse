using BlogVerse.Models;
using X.PagedList;

namespace BlogVerse.ViewModels
{
    public class HomeViewModel
    {
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IPagedList<Post>? Posts { get; set; }
    }
}
