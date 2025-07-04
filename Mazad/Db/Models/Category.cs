using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mazad.Core.Shared.Entities;

namespace Mazad.Models
{
    public class Category : BaseEntity<int>
    {
        [MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(100)]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string DescriptionEn { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string DescriptionAr { get; set; } = string.Empty;
        public int? ParentId { get; set; }

        [MaxLength(2000)]
        public string ImageUrl { get; set; } = string.Empty;
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = [];
        public ICollection<Offer> Offers { get; set; } = [];
    }
}
