﻿namespace BlogVerse.ViewModels
{
    public class SettingViewModel
    {
        public int Id { get; set; }
        public string? SiteName { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? LinkdenUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? GithubUrl { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
