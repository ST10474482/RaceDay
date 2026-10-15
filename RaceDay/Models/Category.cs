using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public ICollection<Event> Events { get; set; }
            = new List<Event>();
    }
}