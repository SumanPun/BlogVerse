using System.ComponentModel.DataAnnotations.Schema;

namespace BlogVerse.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
    }
}
