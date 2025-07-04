using Mazad.Core.Shared.Entities;

namespace Mazad.Core.Domain.Regions;

public class Region : BaseEntity<int>
{
    public string NameArabic { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;

    public int? ParentId { get; set; }
    public Region? ParentRegion { get; set; }
    public ICollection<Region> SubRegions { get; set; } = [];
}
